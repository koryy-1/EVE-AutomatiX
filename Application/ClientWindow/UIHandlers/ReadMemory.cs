﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Configuration;

namespace Application.ClientWindow.UIHandlers
{
    public class EveOnline64
    {
        static public IImmutableList<ulong> EnumeratePossibleAddressesForUIRootObjectsFromProcessId(int processId)
        {
            var memoryReader = new MemoryReaderFromLiveProcess(processId);

            var (committedMemoryRegions, _) = ReadCommittedMemoryRegionsWithoutContentFromProcessId(processId);

            return EnumeratePossibleAddressesForUIRootObjects(committedMemoryRegions, memoryReader);
        }

        static public (IImmutableList<(ulong baseAddress, byte[] content)> memoryRegions, IImmutableList<string> logEntries) ReadCommittedMemoryRegionsWithContentFromProcessId(int processId)
        {
            var genericResult = ReadCommittedMemoryRegionsFromProcessId(processId, readContent: true);

            var memoryRegions =
                genericResult.memoryRegions
                .Select(memoryRegion => (baseAddress: memoryRegion.baseAddress, content: memoryRegion.content))
                .ToImmutableList();

            return (memoryRegions, genericResult.logEntries);
        }

        static public (IImmutableList<(ulong baseAddress, ulong length)> memoryRegions, IImmutableList<string> logEntries) ReadCommittedMemoryRegionsWithoutContentFromProcessId(int processId)
        {
            var genericResult = ReadCommittedMemoryRegionsFromProcessId(processId, readContent: false);

            var memoryRegions =
                genericResult.memoryRegions
                .Select(memoryRegion => (baseAddress: memoryRegion.baseAddress, length: memoryRegion.length))
                .ToImmutableList();

            return (memoryRegions, genericResult.logEntries);
        }

        static public (IImmutableList<(ulong baseAddress, ulong length, byte[] content)> memoryRegions, IImmutableList<string> logEntries) ReadCommittedMemoryRegionsFromProcessId(
            int processId,
            bool readContent)
        {
            var logEntries = new List<string>();

            void logLine(string lineText)
            {
                logEntries.Add(lineText);
                //  Console.WriteLine(lineText);
            }

            logLine("Reading from process " + processId + ".");

            var processHandle = WinApi.OpenProcess(
                (int)(WinApi.ProcessAccessFlags.QueryInformation | WinApi.ProcessAccessFlags.VirtualMemoryRead), false, processId);

            long address = 0;

            var committedRegions = new List<(ulong baseAddress, ulong length, byte[] content)>();

            do
            {
                WinApi.MEMORY_BASIC_INFORMATION64 m;
                int result = WinApi.VirtualQueryEx(processHandle, (IntPtr)address, out m, (uint)Marshal.SizeOf(typeof(WinApi.MEMORY_BASIC_INFORMATION64)));

                var regionProtection = (WinApi.MemoryInformationProtection)m.Protect;

                logLine($"{m.BaseAddress}-{(uint)m.BaseAddress + (uint)m.RegionSize - 1} : {m.RegionSize} bytes result={result}, state={(WinApi.MemoryInformationState)m.State}, type={(WinApi.MemoryInformationType)m.Type}, protection={regionProtection}");

                if (address == (long)m.BaseAddress + (long)m.RegionSize)
                    break;

                address = (long)m.BaseAddress + (long)m.RegionSize;

                if (m.State != (int)WinApi.MemoryInformationState.MEM_COMMIT)
                    continue;

                var protectionFlagsToSkip = WinApi.MemoryInformationProtection.PAGE_GUARD | WinApi.MemoryInformationProtection.PAGE_NOACCESS;

                var matchingFlagsToSkip = protectionFlagsToSkip & regionProtection;

                if (matchingFlagsToSkip != 0)
                {
                    logLine($"Skipping region beginning at {m.BaseAddress:X} as it has flags {matchingFlagsToSkip}.");
                    continue;
                }

                var regionBaseAddress = m.BaseAddress;

                byte[] regionContent = null;

                if (readContent)
                {
                    UIntPtr bytesRead = UIntPtr.Zero;
                    var regionContentBuffer = new byte[(long)m.RegionSize];

                    WinApi.ReadProcessMemory(processHandle, regionBaseAddress, regionContentBuffer, (UIntPtr)regionContentBuffer.LongLength, ref bytesRead);

                    if (bytesRead.ToUInt64() != (ulong)regionContentBuffer.LongLength)
                        throw new Exception($"Failed to ReadProcessMemory at 0x{regionBaseAddress:X}: Only read " + bytesRead + " bytes.");

                    regionContent = regionContentBuffer;
                }

                committedRegions.Add((baseAddress: regionBaseAddress, length: m.RegionSize, content: regionContent));

            } while (true);

            logLine($"Found {committedRegions.Count} committed regions with a total size of {committedRegions.Select(region => (long)region.length).Sum()}.");

            return (committedRegions.ToImmutableList(), logEntries.ToImmutableList());
        }

        static public IImmutableList<ulong> EnumeratePossibleAddressesForUIRootObjects(
            IEnumerable<(ulong baseAddress, ulong length)> memoryRegions,
            IMemoryReader memoryReader)
        {
            var memoryRegionsOrderedByAddress =
                memoryRegions
                .OrderBy(memoryRegion => memoryRegion.baseAddress)
                .ToImmutableList();

            string ReadNullTerminatedAsciiStringFromAddressUpTo255(ulong address)
            {
                var bytesBeforeTruncate = memoryReader.ReadBytes(address, 0x100);

                if (bytesBeforeTruncate == null)
                    return null;

                var bytes =
                    bytesBeforeTruncate
                    .TakeWhile(character => 0 < character)
                    .ToArray();

                return System.Text.Encoding.ASCII.GetString(bytes);
            }

            ulong[] ReadMemoryRegionContentAsULongArray((ulong baseAddress, ulong length) memoryRegion)
            {
                var lengthAsInt32 = (int)memoryRegion.length;

                if ((ulong)lengthAsInt32 != memoryRegion.length)
                    throw new NotSupportedException("Memory region length exceeds supported range: " + memoryRegion.length);

                var asByteArray = memoryReader.ReadBytes(memoryRegion.baseAddress, lengthAsInt32);

                if (asByteArray == null)
                    return null;

                return TransformMemoryContent.AsULongArray(asByteArray);
            }

            IEnumerable<ulong> EnumerateCandidatesForPythonTypeObjectType()
            {
                foreach (var memoryRegion in memoryRegionsOrderedByAddress)
                {
                    var memoryRegionContentAsULongArray = ReadMemoryRegionContentAsULongArray(memoryRegion);

                    if (memoryRegionContentAsULongArray == null)
                        continue;

                    for (var candidateAddressIndex = 0; candidateAddressIndex < memoryRegionContentAsULongArray.Length - 4; ++candidateAddressIndex)
                    {
                        var candidateAddressInProcess = memoryRegion.baseAddress + (ulong)candidateAddressIndex * 8;

                        var candidate_ob_type = memoryRegionContentAsULongArray[candidateAddressIndex + 1];

                        if (candidate_ob_type != candidateAddressInProcess)
                            continue;

                        var candidate_tp_name =
                            ReadNullTerminatedAsciiStringFromAddressUpTo255(memoryRegionContentAsULongArray[candidateAddressIndex + 3]);

                        if (candidate_tp_name != "type")
                            continue;

                        yield return candidateAddressInProcess;
                    }
                }
            }

            IEnumerable<(ulong address, string tp_name)> EnumerateCandidatesForPythonTypeObjects(
                IImmutableList<ulong> typeObjectCandidatesAddresses)
            {
                if (typeObjectCandidatesAddresses.Count < 1)
                    yield break;

                var typeAddressMin = typeObjectCandidatesAddresses.Min();
                var typeAddressMax = typeObjectCandidatesAddresses.Max();

                foreach (var memoryRegion in memoryRegionsOrderedByAddress)
                {
                    var memoryRegionContentAsULongArray = ReadMemoryRegionContentAsULongArray(memoryRegion);

                    if (memoryRegionContentAsULongArray == null)
                        continue;

                    for (var candidateAddressIndex = 0; candidateAddressIndex < memoryRegionContentAsULongArray.Length - 4; ++candidateAddressIndex)
                    {
                        var candidateAddressInProcess = memoryRegion.baseAddress + (ulong)candidateAddressIndex * 8;

                        var candidate_ob_type = memoryRegionContentAsULongArray[candidateAddressIndex + 1];

                        {
                            //  This check is redundant with the following one. It just implements a specialization to optimize runtime expenses.
                            if (candidate_ob_type < typeAddressMin || typeAddressMax < candidate_ob_type)
                                continue;
                        }

                        if (!typeObjectCandidatesAddresses.Contains(candidate_ob_type))
                            continue;

                        var candidate_tp_name =
                            ReadNullTerminatedAsciiStringFromAddressUpTo255(memoryRegionContentAsULongArray[candidateAddressIndex + 3]);

                        if (candidate_tp_name == null)
                            continue;

                        yield return (candidateAddressInProcess, candidate_tp_name);
                    }
                }
            }

            IEnumerable<ulong> EnumerateCandidatesForInstancesOfPythonType(
                IImmutableList<ulong> typeObjectCandidatesAddresses)
            {
                if (typeObjectCandidatesAddresses.Count < 1)
                    yield break;

                var typeAddressMin = typeObjectCandidatesAddresses.Min();
                var typeAddressMax = typeObjectCandidatesAddresses.Max();

                foreach (var memoryRegion in memoryRegionsOrderedByAddress)
                {
                    var memoryRegionContentAsULongArray = ReadMemoryRegionContentAsULongArray(memoryRegion);

                    if (memoryRegionContentAsULongArray == null)
                        continue;

                    for (var candidateAddressIndex = 0; candidateAddressIndex < memoryRegionContentAsULongArray.Length - 4; ++candidateAddressIndex)
                    {
                        var candidateAddressInProcess = memoryRegion.baseAddress + (ulong)candidateAddressIndex * 8;

                        var candidate_ob_type = memoryRegionContentAsULongArray[candidateAddressIndex + 1];

                        {
                            //  This check is redundant with the following one. It just implements a specialization to reduce processing time.
                            if (candidate_ob_type < typeAddressMin || typeAddressMax < candidate_ob_type)
                                continue;
                        }

                        if (!typeObjectCandidatesAddresses.Contains(candidate_ob_type))
                            continue;

                        yield return candidateAddressInProcess;
                    }
                }
            }

            var uiRootTypeObjectCandidatesAddresses =
                EnumerateCandidatesForPythonTypeObjects(EnumerateCandidatesForPythonTypeObjectType().ToImmutableList())
                .Where(typeObject => typeObject.tp_name == "UIRoot")
                .Select(typeObject => typeObject.address)
                .ToImmutableList();

            return
                EnumerateCandidatesForInstancesOfPythonType(uiRootTypeObjectCandidatesAddresses)
                .ToImmutableList();
        }

        struct PyDictEntry
        {
            public ulong hash;
            public ulong key;
            public ulong value;
        }

        static readonly IImmutableSet<string> DictEntriesOfInterestKeys = ImmutableHashSet.Create(
            "_top", "_left", "_width", "_height", "_displayX", "_displayY", 
            "_displayHeight", "_displayWidth",
            "_name", "_text", "_setText",
            "children",
            "texturePath", "_bgTexturePath",
            "_hint", "_display",

            //  HPGauges
            "lastShield", "lastArmor", "lastStructure",

            //  Found in "ShipHudSpriteGauge"
            "_lastValue",

            //  Found in "ModuleButton"
            "ramp_active",

            //  Found in the Transforms contained in "ShipModuleButtonRamps"
            "_rotation",

            //  Found under OverviewEntry in Sprite named "iconSprite"
            "_color",

            //  Found in "SE_TextlineCore"
            "_sr",

            //  Found in "_sr" Bunch
            "htmlstr"
        );

        struct LocalMemoryReadingTools
        {
            public IMemoryReader memoryReader;

            public Func<ulong, IImmutableDictionary<string, ulong>> getDictionaryEntriesWithStringKeys;

            public Func<ulong, string> GetPythonTypeNameFromPythonObjectAddress;

            public Func<ulong, object> GetDictEntryValueRepresentation;
        }

        static readonly IImmutableDictionary<string, Func<ulong, LocalMemoryReadingTools, object>> specializedReadingFromPythonType =
            ImmutableDictionary<string, Func<ulong, LocalMemoryReadingTools, object>>.Empty
            .Add("str", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                return ReadPythonStringValue(address, memoryReadingTools.memoryReader, 0x1000);
            }))
            .Add("unicode", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                var pythonObjectMemory = memoryReadingTools.memoryReader.ReadBytes(address, 0x20);

                if (!(pythonObjectMemory?.Length == 0x20))
                    return "Failed to read python object memory.";

                var unicode_string_length = BitConverter.ToUInt64(pythonObjectMemory, 0x10);

                if (0x1000 < unicode_string_length)
                    return "String too long.";

                var stringBytesCount = (int)unicode_string_length * 2;

                var stringBytes = memoryReadingTools.memoryReader.ReadBytes(BitConverter.ToUInt64(pythonObjectMemory, 0x18), stringBytesCount);

                if (!(stringBytes?.Length == (int)stringBytesCount))
                    return "Failed to read string bytes.";

                return System.Text.Encoding.Unicode.GetString(stringBytes, 0, stringBytes.Length);
            }))
            .Add("int", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                var intObjectMemory = memoryReadingTools.memoryReader.ReadBytes(address, 0x18);

                if (!(intObjectMemory?.Length == 0x18))
                    return "Failed to read int object memory.";

                var value = BitConverter.ToInt64(intObjectMemory, 0x10);

                var asInt32 = (Int32)value;

                if (asInt32 == value)
                    return asInt32;

                return new PyInt
                {
                    @int = value,
                    int_low32 = asInt32,
                };
            }))
            .Add("bool", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                var pythonObjectMemory = memoryReadingTools.memoryReader.ReadBytes(address, 0x18);

                if (!(pythonObjectMemory?.Length == 0x18))
                    return "Failed to read python object memory.";

                return BitConverter.ToInt64(pythonObjectMemory, 0x10) != 0;
            }))
            .Add("float", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                return ReadPythonFloatObjectValue(address, memoryReadingTools.memoryReader);
            }))
            .Add("PyColor", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                var pyColorObjectMemory = memoryReadingTools.memoryReader.ReadBytes(address, 0x18);

                if (!(pyColorObjectMemory?.Length == 0x18))
                    return "Failed to read pyColorObjectMemory.";

                var dictionaryAddress = BitConverter.ToUInt64(pyColorObjectMemory, 0x10);

                var dictionaryEntries = memoryReadingTools.getDictionaryEntriesWithStringKeys(dictionaryAddress);

                if (dictionaryEntries == null)
                    return "Failed to read dictionary entries.";

                int? readValuePercentFromDictEntryKey(string dictEntryKey)
                {
                    if (!dictionaryEntries.TryGetValue(dictEntryKey, out var valueAddress))
                        return null;

                    var valueAsFloat = ReadPythonFloatObjectValue(valueAddress, memoryReadingTools.memoryReader);

                    if (!valueAsFloat.HasValue)
                        return null;

                    return (int)(valueAsFloat.Value * 100);
                }

                return new PyColor
                {
                    aPercent = readValuePercentFromDictEntryKey("_a"),
                    rPercent = readValuePercentFromDictEntryKey("_r"),
                    gPercent = readValuePercentFromDictEntryKey("_g"),
                    bPercent = readValuePercentFromDictEntryKey("_b"),
                };
            }))
            .Add("Bunch", new Func<ulong, LocalMemoryReadingTools, object>((address, memoryReadingTools) =>
            {
                var dictionaryEntries = memoryReadingTools.getDictionaryEntriesWithStringKeys(address);

                if (dictionaryEntries == null)
                    return "Failed to read dictionary entries.";

                var entriesOfInterest = new List<UITreeNode.DictEntry>();

                foreach (var entry in dictionaryEntries)
                {
                    if (!DictEntriesOfInterestKeys.Contains(entry.Key))
                    {
                        continue;
                    }

                    entriesOfInterest.Add(new UITreeNode.DictEntry
                    {
                        key = entry.Key,
                        value = memoryReadingTools.GetDictEntryValueRepresentation(entry.Value)
                    });
                }

                var entriesOfInterestJObject =
                    new Newtonsoft.Json.Linq.JObject(
                        entriesOfInterest.Select(dictEntry =>
                            new Newtonsoft.Json.Linq.JProperty(dictEntry.key, Newtonsoft.Json.Linq.JToken.FromObject(dictEntry.value))));

                return new UITreeNode.Bunch
                {
                    entriesOfInterest = entriesOfInterestJObject,
                };
            }));

        class MemoryReadingCache
        {
            IDictionary<ulong, string> PythonTypeNameFromPythonObjectAddress;

            IDictionary<ulong, string> PythonStringValueMaxLength4000;

            IDictionary<ulong, object> DictEntryValueRepresentation;

            public MemoryReadingCache()
            {
                PythonTypeNameFromPythonObjectAddress = new Dictionary<ulong, string>();
                PythonStringValueMaxLength4000 = new Dictionary<ulong, string>();
                DictEntryValueRepresentation = new Dictionary<ulong, object>();
            }

            public string GetPythonTypeNameFromPythonObjectAddress(ulong address, Func<ulong, string> getFresh) =>
                GetFromCacheOrUpdate(PythonTypeNameFromPythonObjectAddress, address, getFresh);

            public string GetPythonStringValueMaxLength4000(ulong address, Func<ulong, string> getFresh) =>
                GetFromCacheOrUpdate(PythonStringValueMaxLength4000, address, getFresh);

            public object GetDictEntryValueRepresentation(ulong address, Func<ulong, object> getFresh) =>
                GetFromCacheOrUpdate(DictEntryValueRepresentation, address, getFresh);

            static TValue GetFromCacheOrUpdate<TKey, TValue>(IDictionary<TKey, TValue> cache, TKey key, Func<TKey, TValue> getFresh)
            {
                if (cache.TryGetValue(key, out var fromCache))
                    return fromCache;

                var fresh = getFresh(key);

                cache[key] = fresh;
                return fresh;
            }
        }

        static public UITreeNode ReadUITreeFromAddress(ulong nodeAddress, IMemoryReader memoryReader, int maxDepth) =>
            ReadUITreeFromAddress(nodeAddress, memoryReader, maxDepth, null);

        static UITreeNode ReadUITreeFromAddress(ulong nodeAddress, IMemoryReader memoryReader, int maxDepth, MemoryReadingCache cache)
        {
            cache = cache ?? new MemoryReadingCache();

            var uiNodeObjectMemory = memoryReader.ReadBytes(nodeAddress, 0x30);

            if (!(0x30 == uiNodeObjectMemory?.Length))
                return null;

            string getPythonTypeNameFromPythonTypeObjectAddress(ulong typeObjectAddress)
            {
                var typeObjectMemory = memoryReader.ReadBytes(typeObjectAddress, 0x20);

                if (!(typeObjectMemory?.Length == 0x20))
                    return null;

                var tp_name = BitConverter.ToUInt64(typeObjectMemory, 0x18);

                var nameBytes = memoryReader.ReadBytes(tp_name, 100);

                if (!(nameBytes?.Contains((byte)0) ?? false))
                    return null;

                return System.Text.Encoding.ASCII.GetString(nameBytes.TakeWhile(character => character != 0).ToArray());
            }

            string getPythonTypeNameFromPythonObjectAddress(ulong objectAddress)
            {
                return cache.GetPythonTypeNameFromPythonObjectAddress(objectAddress, objectAddress_arg =>
                {
                    var objectMemory = memoryReader.ReadBytes(objectAddress_arg, 0x10);

                    if (!(objectMemory?.Length == 0x10))
                        return null;

                    return getPythonTypeNameFromPythonTypeObjectAddress(BitConverter.ToUInt64(objectMemory, 8));
                });
            }

            string readPythonStringValueMaxLength4000(ulong strObjectAddress)
            {
                return cache.GetPythonStringValueMaxLength4000(
                    strObjectAddress,
                    strObjectAddress_arg => ReadPythonStringValue(strObjectAddress_arg, memoryReader, 4000));
            }

            PyDictEntry[] ReadActiveDictionaryEntriesFromDictionaryAddress(ulong dictionaryAddress)
            {
                /*
                Sources:
                https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Include/dictobject.h
                https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Objects/dictobject.c
                */

                var dictMemory = memoryReader.ReadBytes(dictionaryAddress, 0x30);

                //  Console.WriteLine($"dictMemory is {(dictMemory == null ? "not " : "")}ok for 0x{dictionaryAddress:X}");

                if (!(dictMemory?.Length == 0x30))
                    return null;

                var dictMemoryAsLongArray = TransformMemoryContent.AsULongArray(dictMemory);

                //  var dictTypeName = getPythonTypeNameFromObjectAddress(dictionaryAddress);

                //  Console.WriteLine($"Type name for dictionary 0x{dictionaryAddress:X} is '{dictTypeName}'.");

                //  https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Include/dictobject.h#L60-L89

                var ma_fill = dictMemoryAsLongArray[2];
                var ma_used = dictMemoryAsLongArray[3];
                var ma_mask = dictMemoryAsLongArray[4];
                var ma_table = dictMemoryAsLongArray[5];

                //  Console.WriteLine($"Details for dictionary 0x{dictionaryAddress:X}: type_name = '{dictTypeName}' ma_mask = 0x{ma_mask:X}, ma_table = 0x{ma_table:X}.");

                var numberOfSlots = (int)ma_mask + 1;

                if (numberOfSlots < 0 || 10_000 < numberOfSlots)
                {
                    //  Avoid stalling the whole reading process when a single dictionary contains garbage.
                    return null;
                }

                var slotsMemorySize = numberOfSlots * 8 * 3;

                var slotsMemory = memoryReader.ReadBytes(ma_table, slotsMemorySize);

                //  Console.WriteLine($"slotsMemory (0x{ma_table:X}) has length of {slotsMemory?.Length} and is {(slotsMemory?.Length == slotsMemorySize ? "" : "not ")}ok for 0x{dictionaryAddress:X}");

                if (!(slotsMemory?.Length == slotsMemorySize))
                    return null;

                var slotsMemoryAsLongArray = TransformMemoryContent.AsULongArray(slotsMemory);

                var entries = new List<PyDictEntry>();

                for (var slotIndex = 0; slotIndex < numberOfSlots; ++slotIndex)
                {
                    var hash = slotsMemoryAsLongArray[slotIndex * 3];
                    var key = slotsMemoryAsLongArray[slotIndex * 3 + 1];
                    var value = slotsMemoryAsLongArray[slotIndex * 3 + 2];

                    if (key == 0 || value == 0)
                        continue;

                    entries.Add(new PyDictEntry { hash = hash, key = key, value = value });
                }

                return entries.ToArray();
            }

            IImmutableDictionary<string, ulong> GetDictionaryEntriesWithStringKeys(ulong dictionaryObjectAddress)
            {
                var dictionaryEntries_1 = ReadActiveDictionaryEntriesFromDictionaryAddress(dictionaryObjectAddress);

                if (dictionaryEntries_1 == null)
                    return null;

                return
                    dictionaryEntries_1.ToImmutableDictionary(
                        entry => readPythonStringValueMaxLength4000(entry.key),
                        entry => entry.value);
            }

            var localMemoryReadingTools = new LocalMemoryReadingTools
            {
                memoryReader = memoryReader,
                getDictionaryEntriesWithStringKeys = GetDictionaryEntriesWithStringKeys,
                GetPythonTypeNameFromPythonObjectAddress = getPythonTypeNameFromPythonObjectAddress,
            };

            var pythonObjectTypeName = getPythonTypeNameFromPythonObjectAddress(nodeAddress);

            if (!(0 < pythonObjectTypeName?.Length))
                return null;

            var dictAddress = BitConverter.ToUInt64(uiNodeObjectMemory, 0x10);

            var dictionaryEntries = ReadActiveDictionaryEntriesFromDictionaryAddress(dictAddress);

            if (dictionaryEntries == null)
                return null;

            var dictEntriesOfInterest = new List<UITreeNode.DictEntry>();

            var otherDictEntriesKeys = new List<string>();

            object GetDictEntryValueRepresentation(ulong valueOjectAddress)
            {
                return cache.GetDictEntryValueRepresentation(valueOjectAddress, valueOjectAddress_arg =>
                {
                    var genericRepresentation = new UITreeNode.DictEntryValueGenericRepresentation
                    {
                        address = valueOjectAddress_arg,
                        pythonObjectTypeName = null
                    };

                    var value_pythonTypeName = getPythonTypeNameFromPythonObjectAddress(valueOjectAddress_arg);

                    genericRepresentation.pythonObjectTypeName = value_pythonTypeName;

                    if (value_pythonTypeName == null)
                        return genericRepresentation;

                    specializedReadingFromPythonType.TryGetValue(value_pythonTypeName, out var specializedRepresentation);

                    if (specializedRepresentation == null)
                        return genericRepresentation;

                    return specializedRepresentation(genericRepresentation.address, localMemoryReadingTools);
                });
            }

            localMemoryReadingTools.GetDictEntryValueRepresentation = GetDictEntryValueRepresentation;

            foreach (var dictionaryEntry in dictionaryEntries)
            {
                var keyObject_type_name = getPythonTypeNameFromPythonObjectAddress(dictionaryEntry.key);

                //  Console.WriteLine($"Dict entry type name is '{keyObject_type_name}'");

                if (keyObject_type_name != "str")
                    continue;

                var keyString = readPythonStringValueMaxLength4000(dictionaryEntry.key);

                if (!DictEntriesOfInterestKeys.Contains(keyString))
                {
                    otherDictEntriesKeys.Add(keyString);
                    continue;
                }

                //if (keyString == "_setText" || keyString == "_text")
                //    Console.WriteLine("TEXT IN UI " + keyString + ": " + GetDictEntryValueRepresentation(dictionaryEntry.value));

                dictEntriesOfInterest.Add(new UITreeNode.DictEntry
                {
                    key = keyString,
                    value = GetDictEntryValueRepresentation(dictionaryEntry.value)
                });
            }

            {
                var _displayDictEntry = dictEntriesOfInterest.FirstOrDefault(c => c.key == "_display");

                if (_displayDictEntry != null && (_displayDictEntry.value is bool displayAsBool))
                    if (!displayAsBool)
                        return null;
            }

            UITreeNode[] ReadChildren()
            {
                if (maxDepth < 1)
                    return null;

                //  https://github.com/Arcitectus/Sanderling/blob/b07769fb4283e401836d050870121780f5f37910/guide/image/2015-01.eve-online-python-ui-tree-structure.png

                var childrenDictEntry = dictEntriesOfInterest.FirstOrDefault(c => c.key == "children");

                if (childrenDictEntry == null)
                    return null;

                ulong childrenEntryObjectAddress;
                try
                {
                    childrenEntryObjectAddress =
                    ((UITreeNode.DictEntryValueGenericRepresentation)childrenDictEntry.value).address;
                }
                catch (Exception)
                {
                    //Console.WriteLine("Unable to cast object of type '<>f__AnonymousType1 to type 'DictEntryValueGenericRepresentation'.");
                    //Console.WriteLine(childrenDictEntry.value);
                    return null;
                }
                
                //  Console.WriteLine($"'children' dict entry of 0x{nodeAddress:X} points to 0x{childrenEntryObjectAddress:X}.");

                var pyChildrenListMemory = memoryReader.ReadBytes(childrenEntryObjectAddress, 0x18);

                if (!(pyChildrenListMemory?.Length == 0x18))
                    return null;

                var pyChildrenDictAddress = BitConverter.ToUInt64(pyChildrenListMemory, 0x10);

                var pyChildrenDictEntries = ReadActiveDictionaryEntriesFromDictionaryAddress(pyChildrenDictAddress);

                //  Console.WriteLine($"Found {(pyChildrenDictEntries == null ? "no" : "some")} children dictionary entries for 0x{nodeAddress:X}");

                if (pyChildrenDictEntries == null)
                    return null;

                var childrenEntry =
                    pyChildrenDictEntries
                    .FirstOrDefault(dictionaryEntry =>
                    {
                        if (getPythonTypeNameFromPythonObjectAddress(dictionaryEntry.key) != "str")
                            return false;

                        var keyString = readPythonStringValueMaxLength4000(dictionaryEntry.key);

                        return keyString == "_childrenObjects";
                    });

                //  Console.WriteLine($"Found {(childrenEntry.value == 0 ? "no" : "a")} dictionary entry for children of 0x{nodeAddress:X}");

                if (childrenEntry.value == 0)
                    return null;

                var pythonListObjectMemory = memoryReader.ReadBytes(childrenEntry.value, 0x20);

                if (!(pythonListObjectMemory?.Length == 0x20))
                    return null;

                //  https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Include/listobject.h

                var list_ob_size = BitConverter.ToUInt64(pythonListObjectMemory, 0x10);

                if (4000 < list_ob_size)
                    return null;

                var listEntriesSize = (int)list_ob_size * 8;

                var list_ob_item = BitConverter.ToUInt64(pythonListObjectMemory, 0x18);

                var listEntriesMemory = memoryReader.ReadBytes(list_ob_item, listEntriesSize);

                if (!(listEntriesMemory?.Length == listEntriesSize))
                    return null;

                var listEntries = TransformMemoryContent.AsULongArray(listEntriesMemory);

                //  Console.WriteLine($"Found {listEntries.Length} children entries for 0x{nodeAddress:X}: " + String.Join(", ", listEntries.Select(childAddress => $"0x{childAddress:X}").ToArray()));

                return
                     listEntries
                     .Select(childAddress => ReadUITreeFromAddress(childAddress, memoryReader, maxDepth - 1, cache))
                     .ToArray();
            }

            var dictEntriesOfInterestLessNoneType =
                dictEntriesOfInterest
                .Where(c => !(((object)c.value as UITreeNode.DictEntryValueGenericRepresentation)?.pythonObjectTypeName == "NoneType"))
                .ToArray();

            var dictEntriesOfInterestJObject =
                new Newtonsoft.Json.Linq.JObject(
                    dictEntriesOfInterestLessNoneType.Select(dictEntry =>
                        new Newtonsoft.Json.Linq.JProperty(dictEntry.key, Newtonsoft.Json.Linq.JToken.FromObject(dictEntry.value))));

            return new UITreeNode
            {
                pythonObjectAddress = nodeAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterestJObject,
                otherDictEntriesKeys = otherDictEntriesKeys.ToArray(),
                children = ReadChildren()?.Where(child => child != null)?.ToArray(),
            };
        }

        static string ReadPythonStringValue(ulong stringObjectAddress, IMemoryReader memoryReader, int maxLength)
        {
            //  https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Include/stringobject.h

            var stringObjectMemory = memoryReader.ReadBytes(stringObjectAddress, 0x20);

            if (!(stringObjectMemory?.Length == 0x20))
                return "Failed to read string object memory.";

            var stringObject_ob_size = BitConverter.ToUInt64(stringObjectMemory, 0x10);

            if (0 < maxLength && maxLength < (int)stringObject_ob_size || int.MaxValue < stringObject_ob_size)
                return "String too long.";

            var stringBytes = memoryReader.ReadBytes(stringObjectAddress + 8 * 4, (int)stringObject_ob_size);

            if (!(stringBytes?.Length == (int)stringObject_ob_size))
                return "Failed to read string bytes.";

            return System.Text.Encoding.ASCII.GetString(stringBytes, 0, stringBytes.Length);
        }

        static double? ReadPythonFloatObjectValue(ulong floatObjectAddress, IMemoryReader memoryReader)
        {
            //  https://github.com/python/cpython/blob/362ede2232107fc54d406bb9de7711ff7574e1d4/Include/floatobject.h

            var pythonObjectMemory = memoryReader.ReadBytes(floatObjectAddress, 0x20);

            if (!(pythonObjectMemory?.Length == 0x20))
                return null;

            return BitConverter.ToDouble(pythonObjectMemory, 0x10);
        }
    }


    public interface IMemoryReader
    {
        byte[] ReadBytes(ulong startAddress, int length);
    }

    public class MemoryReaderFromProcessSample : IMemoryReader
    {
        readonly IImmutableList<(ulong baseAddress, byte[] content)> memoryRegionsOrderedByAddress;

        public MemoryReaderFromProcessSample(IImmutableList<(ulong baseAddress, byte[] content)> memoryRegions)
        {
            memoryRegionsOrderedByAddress =
                memoryRegions
                .OrderBy(memoryRegion => memoryRegion.baseAddress)
                .ToImmutableList();
        }

        public byte[] ReadBytes(ulong startAddress, int length)
        {
            var memoryRegion =
                memoryRegionsOrderedByAddress
                .Where(c => c.baseAddress <= startAddress)
                .OrderBy(c => c.baseAddress)
                .LastOrDefault();

            if (memoryRegion.content == null)
                return null;

            return
                memoryRegion.content
                .Skip((int)(startAddress - memoryRegion.baseAddress))
                .Take(length)
                .ToArray();
        }
    }


    public class MemoryReaderFromLiveProcess : IMemoryReader, IDisposable
    {
        IntPtr processHandle;

        public MemoryReaderFromLiveProcess(int processId)
        {
            processHandle = WinApi.OpenProcess(
                (int)(WinApi.ProcessAccessFlags.QueryInformation | WinApi.ProcessAccessFlags.VirtualMemoryRead), false, processId);
        }

        public void Dispose()
        {
            if (processHandle != IntPtr.Zero)
                WinApi.CloseHandle(processHandle);
        }

        public byte[] ReadBytes(ulong startAddress, int length)
        {
            var buffer = new byte[length];

            UIntPtr numberOfBytesReadAsPtr = UIntPtr.Zero;

            if (!WinApi.ReadProcessMemory(processHandle, startAddress, buffer, (UIntPtr)buffer.LongLength, ref numberOfBytesReadAsPtr))
                return null;

            var numberOfBytesRead = numberOfBytesReadAsPtr.ToUInt64();

            if (numberOfBytesRead == 0)
                return null;

            if (int.MaxValue < numberOfBytesRead)
                return null;

            if (numberOfBytesRead == (ulong)buffer.LongLength)
                return buffer;

            return buffer.AsSpan(0, (int)numberOfBytesRead).ToArray();
        }
    }

    static public class WinApi
    {
        [DllImport("kernel32.dll")]
        static public extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static public extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        static public extern bool ReadProcessMemory(IntPtr hProcess, ulong lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, ref UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern bool CloseHandle(IntPtr hHandle);

        [DllImport("Kernel32.dll")]
        static public extern int GetProcessId(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static public extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        static public extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        static public extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        static public extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static public extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        static public extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        static public int MakeLParam(int LoWord, int HiWord)
        {
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;

            public int y;
        }

        //  http://www.pinvoke.net/default.aspx/kernel32.virtualqueryex
        //  https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION64
        {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public int State;
            public int Protect;
            public int Type;
            public int __alignment2;
        }

        public enum AllocationProtect : uint
        {
            PAGE_EXECUTE = 0x00000010,
            PAGE_EXECUTE_READ = 0x00000020,
            PAGE_EXECUTE_READWRITE = 0x00000040,
            PAGE_EXECUTE_WRITECOPY = 0x00000080,
            PAGE_NOACCESS = 0x00000001,
            PAGE_READONLY = 0x00000002,
            PAGE_READWRITE = 0x00000004,
            PAGE_WRITECOPY = 0x00000008,
            PAGE_GUARD = 0x00000100,
            PAGE_NOCACHE = 0x00000200,
            PAGE_WRITECOMBINE = 0x00000400
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        //  https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information
        public enum MemoryInformationState : int
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000,
        }

        //  https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information
        public enum MemoryInformationType : int
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000,
        }

        //  https://docs.microsoft.com/en-au/windows/win32/memory/memory-protection-constants
        [Flags]
        public enum MemoryInformationProtection : int
        {
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000,

            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
        }

        public enum MouseMessages : uint
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
        }

        public enum KeyboardMessages : uint
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
        }

        public enum VirtualKeyShort : int
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            VK_CONTROL = 0x11,
            VK_F = 0x46,
            VK_G = 0x47,
            VK_H = 0x48,
            VK_V = 0x56,
            VK_R = 0x52,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_UP = 0x26,
            VK_DOWN = 0x28,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
        }

    }

    /// <summary>
    /// Offsets from https://docs.python.org/2/c-api/structures.html
    /// </summary>
    public class PyObject
    {
        public const int Offset_ob_refcnt = 0;
        public const int Offset_ob_type = 8;
    }

    public class PyInt
    {
        public long @int { set; get; }
        public int int_low32 { set; get; }
    }

    public class PyColor
    {
        public int? aPercent { set; get; }
        public int? rPercent { set; get; }
        public int? gPercent { set; get; }
        public int? bPercent { set; get; }
    }

    public class UITreeNode
    {
        public ulong pythonObjectAddress { set; get; }

        public string pythonObjectTypeName { set; get; }

        public Newtonsoft.Json.Linq.JObject dictEntriesOfInterest { set; get; }

        public string[] otherDictEntriesKeys { set; get; }

        public UITreeNode[] children { set; get; }

        public class DictEntryValueGenericRepresentation
        {
            public ulong address { set; get; }

            public string pythonObjectTypeName { set; get; }
        }

        public class DictEntry
        {
            public string key { set; get; }
            public object value { set; get; }
        }

        public class Bunch
        {
            public Newtonsoft.Json.Linq.JObject entriesOfInterest { set; get; }
        }

        public IEnumerable<UITreeNode> EnumerateSelfAndDescendants() =>
            new[] { this }
            .Concat((children ?? Array.Empty<UITreeNode>()).SelectMany(child => child?.EnumerateSelfAndDescendants() ?? ImmutableList<UITreeNode>.Empty));

        public UITreeNode WithOtherDictEntriesRemoved()
        {
            return new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = null,
                children = children?.Select(child => child?.WithOtherDictEntriesRemoved()).ToArray(),
            };
        }

        public UITreeNode handleEntity(string str)
        {
            UITreeNode Entity = new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = otherDictEntriesKeys,
                children = children,
            };
            if (Entity.dictEntriesOfInterest.ContainsKey("needIndex"))
            {
                if (Entity.children[Convert.ToInt32(Entity.dictEntriesOfInterest["needIndex"].ToString())].pythonObjectTypeName == str)
                {
                    return Entity;
                }
                Entity = Entity.children[Convert.ToInt32(Entity.dictEntriesOfInterest["needIndex"].ToString())].handleEntity(str);
            }
            return Entity;
        }

        public UITreeNode handleEntityByDictEntriesOfInterest(string key, string value)
        {
            UITreeNode Entity = new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = otherDictEntriesKeys,
                children = children,
            };
            if (Entity.dictEntriesOfInterest.ContainsKey("needIndex"))
            {
                if (Entity.children[Convert.ToInt32(Entity.dictEntriesOfInterest["needIndex"].ToString())]
                    .dictEntriesOfInterest.ContainsKey(key))
                {
                    if (Entity.children[Convert.ToInt32(Entity.dictEntriesOfInterest["needIndex"].ToString())]
                    .dictEntriesOfInterest[key].ToString().Contains(value))
                    {
                        return Entity;
                    }
                }
                Entity = Entity.children[Convert.ToInt32(Entity.dictEntriesOfInterest["needIndex"].ToString())]
                    .handleEntityByDictEntriesOfInterest(key, value);
            }
            return Entity;
        }

        public UITreeNode FindEntityOfString(string str)
        {
            int needIndex = 0;
            UITreeNode needEntity =  new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = otherDictEntriesKeys,
                children = children
                 
            };
            needEntity.children = needEntity.children?
                .Select(child =>
                {
                    if (child != null)
                    {
                        if (child.pythonObjectTypeName == str)
                            return child;

                        if (child.children != null)
                            return child.FindEntityOfString(str);
                    }
                    return null;
                })
                .ToArray();
            foreach (var child1 in needEntity.children)
            {
                if (child1 == null)
                    needIndex++;
                else
                    break;
            }
            
            if (!needEntity.dictEntriesOfInterest.ContainsKey("needIndex"))
                needEntity.dictEntriesOfInterest.Add("needIndex", needIndex);
           
            var cntEntities = 0;
            foreach (var child1 in needEntity.children)
            {
                if (child1 != null)
                    cntEntities++;
            }
            if (cntEntities == 0)
                return null;

            return needEntity;
        }

        public UITreeNode FindEntityOfStringByDictEntriesOfInterest(string key, string value)
        {
            int needIndex = 0;
            UITreeNode needEntity = new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = otherDictEntriesKeys,
                children = children

            };
            needEntity.children = needEntity.children?
                .Select(child =>
                {
                    if (child != null)
                    {
                        if (child.dictEntriesOfInterest.ContainsKey(key))
                        {
                            if (child.dictEntriesOfInterest[key].ToString().Contains(value))
                                return child;
                        }
                        if (child.children != null)
                            return child.FindEntityOfStringByDictEntriesOfInterest(key, value);
                    }
                    return null;
                })
                .ToArray();
            foreach (var child1 in needEntity.children)
            {
                if (child1 == null)
                    needIndex++;
                else
                    break;
            }

            if (!needEntity.dictEntriesOfInterest.ContainsKey("needIndex"))
                needEntity.dictEntriesOfInterest.Add("needIndex", needIndex);


            var cntEntities = 0;
            foreach (var child1 in needEntity.children)
            {
                if (child1 != null)
                    cntEntities++;
            }
            if (cntEntities == 0)
                return null;

            return needEntity;
        }

        public UITreeNode GetMarkedChildEntity()
        {
            if (!dictEntriesOfInterest.ContainsKey("needIndex"))
            {
                throw new InvalidOperationException("Key 'needIndex' not found in dictEntriesOfInterest.");
            }
            
            return children[Convert.ToInt32(dictEntriesOfInterest["needIndex"])];
        }

        public bool HasValidChildren(int[] childIndices)
        {
            UITreeNode node = new UITreeNode
            {
                pythonObjectAddress = pythonObjectAddress,
                pythonObjectTypeName = pythonObjectTypeName,
                dictEntriesOfInterest = dictEntriesOfInterest,
                otherDictEntriesKeys = otherDictEntriesKeys,
                children = children
            };
            foreach (int index in childIndices)
            {
                if (node.children == null || node.children.Length <= index || node.children[index] == null)
                    return false;
                node = node.children[index];
            }
            return true;
        }
    }

    static class TransformMemoryContent
    {
        static public ulong[] AsULongArray(byte[] byteArray)
        {
            var ulongArray = new ulong[byteArray.Length / 8];
            Buffer.BlockCopy(byteArray, 0, ulongArray, 0, ulongArray.Length * 8);
            return ulongArray;
        }
    }

    public class IntegersToStringJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;
        public override bool CanConvert(Type type) =>
            type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(ulong);

        public override void WriteJson(
            Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(
            Newtonsoft.Json.JsonReader reader, Type type, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }

    public static class ReadMemory
    {
        static readonly object _locker = new object();

        public static (IImmutableList<ulong>, IMemoryReader) GetRootAddressesAndMemoryReader(string RootAddress, int processId)
        {
            lock (_locker)
            {
                if (processId == 0)
                {
                    throw new NotSupportedException("bad pid or HWND");
                }

                if (0 < RootAddress?.Length)
                {
                    return (ImmutableList.Create(ParseULong(RootAddress)), new MemoryReaderFromLiveProcess(processId));
                }

                var (committedRegions, logEntries) = EveOnline64.ReadCommittedMemoryRegionsWithContentFromProcessId(processId);

                //Console.WriteLine($"Reading from process sample {processSampleId}.");

                var searchUIRootsStopwatch = System.Diagnostics.Stopwatch.StartNew();

                var memoryReader = new MemoryReaderFromProcessSample(committedRegions);

                var memoryRegions =
                    committedRegions
                    .Select(memoryRegion => (memoryRegion.baseAddress, length: (ulong)memoryRegion.content.LongLength))
                    .ToImmutableList();

                var uiRootCandidatesAddresses =
                    EveOnline64.EnumeratePossibleAddressesForUIRootObjects(memoryRegions, memoryReader)
                    .ToImmutableList();

                searchUIRootsStopwatch.Stop();

                //Console.WriteLine($"Found {uiRootCandidatesAddresses.Count} candidates for UIRoot in " +
                //    $"{(int)searchUIRootsStopwatch.Elapsed.TotalSeconds} seconds: " + 
                //    string.Join(",", uiRootCandidatesAddresses.Select(address => $"0x{address:X}")));

                return (uiRootCandidatesAddresses, memoryReader);
            }
        }

        public static UITreeNode GetUITrees(string RootAddress, int processId, int maxDepth = 99)
        {
            var (uiRootCandidatesAddresses, memoryReader) = GetRootAddressesAndMemoryReader(RootAddress, processId);

            IImmutableList<UITreeNode> ReadUITrees() =>
                    uiRootCandidatesAddresses
                    .Select(uiTreeRoot => EveOnline64.ReadUITreeFromAddress(uiTreeRoot, memoryReader, maxDepth))
                    .Where(uiTree => uiTree != null)
                    .ToImmutableList();

            var readUiTreesStopwatch = System.Diagnostics.Stopwatch.StartNew();

            var uiTrees = ReadUITrees();

            readUiTreesStopwatch.Stop();

            var uiTreesWithStats =
                uiTrees
                .Select(uiTree =>
                new
                {
                    uiTree = uiTree,
                    nodeCount = uiTree.EnumerateSelfAndDescendants().Count()
                })
                .OrderByDescending(uiTreeWithStats => uiTreeWithStats.nodeCount)
                .ToImmutableList();

            //var uiTreesReport =
            //    uiTreesWithStats
            //    .Select(uiTreeWithStats => $"\n0x{uiTreeWithStats.uiTree.pythonObjectAddress:X}: " +
            //    $"{uiTreeWithStats.nodeCount} nodes.")
            //    .ToImmutableList();

            //Console.WriteLine($"Read {uiTrees.Count} UI trees in {(int)readUiTreesStopwatch.Elapsed.TotalMilliseconds} milliseconds:" + string.Join("", uiTreesReport));

            var largestUiTree =
                uiTreesWithStats
                .OrderByDescending(uiTreeWithStats => uiTreeWithStats.nodeCount)
                .FirstOrDefault().uiTree;

            if (largestUiTree != null)
            {
                var uiTreePreparedForFile = largestUiTree;

                uiTreePreparedForFile = uiTreePreparedForFile.WithOtherDictEntriesRemoved();

                return uiTreePreparedForFile;
            }
            else
            {
                //Console.WriteLine("No largest UI tree or some shit");
                return null;
            }
        }

        static ulong ParseULong(string asString)
        {
            if (asString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                return ulong.Parse(asString.Substring(2), System.Globalization.NumberStyles.HexNumber);

            return ulong.Parse(asString);
        }
    }
}
