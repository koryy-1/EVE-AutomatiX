using Application.ClientWindow.UIHandlers;
using Domen.dto;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.ClientWindow
{
    public class Client
    {
        public Parser Parser { get; private set; }
        public Emulators Emulators { get; private set; }
        public ClientParams ClientParams { get; private set; }
        public SearchingStatus Status { get; private set; }

        private string _nickName;

        private static Client _instance;
        private static readonly object _lock = new object();

        public Client(string nickName)
        {
            _nickName = nickName;
            ClientParams = PreInitClientParams();

            Status = new SearchingStatus();
        }

        public static Client GetInstance(string nickName)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Client(nickName);
                    }
                }
            }
            return _instance;
        }

        public SearchingStatus CheckRootAddressActuality()
        {
            var isActual = IsActualRootAddress(ClientParams);
            if (isActual)
            {
                Status.IsActualRootAddress = true;
                return Status;
            }
            else
            {
                // както запретить использование Parsers и Emulators
                Status.IsActualRootAddress = false;
                return Status;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newParams"></param>
        /// <returns>success</returns>
        public bool SetClientParams(ClientParams newParams)
        {
            if (!IsActualRootAddress(newParams))
            {
                return false;
            }
            ClientParams = newParams;
            Parser = new Parser(newParams);
            Emulators = new Emulators(newParams.hWnd);

            return true;
        }

        public void StartSearchingRootAddress()
        {
            if (IsActualRootAddress(ClientParams))
            {
                return;
            }

            SetStatusStartSearching();
            var newClientParams = UIRootAddress.UpdateRootAddress(_nickName);
            SetStatusStopSearching();

            SetClientParams(newClientParams);
        }

        public void StopSearching()
        {
            // Логика для остановки поиска рут адреса
            // concelationToken.RequestCancell
            SetStatusInterruptSearching();
            throw new NotImplementedException();
        }

        // proc name / hwnd / pid
        private ClientParams PreInitClientParams()
        {
            return UIRootAddress.PreInitClientParams(_nickName);
        }

        public bool IsActualRootAddress(ClientParams clientParams)
        {
            return UIRootAddress.IsActualRootAddress(clientParams);
        }

        private void SetStatusStartSearching()
        {
            Status.IsSearching = true;
            Status.IsActualRootAddress = false;
        }

        private void SetStatusStopSearching()
        {
            Status.IsSearching = false;
            Status.IsActualRootAddress = true;
        }

        private void SetStatusInterruptSearching()
        {
            Status.IsSearching = false;
            Status.IsActualRootAddress = false;
        }

        //todo: mapping
        public ClientParamsDto ClientParams2Dto(ClientParams clientParams)
        {
            ClientParamsDto clientParamsDto = new ClientParamsDto()
            {
                hWnd = clientParams.hWnd.ToInt32(),
                ProcessId = clientParams.ProcessId,
                ProcessName = clientParams.ProcessName,
                RootAddress = clientParams.RootAddress,
            };
            return clientParamsDto;
        }

        public ClientParams Dto2ClientParams(ClientParamsDto clientParamsDto)
        {
            ClientParams clientParams = new ClientParams()
            {
                hWnd = new IntPtr(clientParamsDto.hWnd),
                ProcessId = clientParamsDto.ProcessId,
                ProcessName = clientParamsDto.ProcessName,
                RootAddress = clientParamsDto.RootAddress,
            };
            return clientParams;
        }
    }
}
