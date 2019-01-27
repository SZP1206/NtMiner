﻿using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelInputViewModels : ViewModelBase {
        public static readonly KernelInputViewModels Current = new KernelInputViewModels();

        private readonly Dictionary<Guid, KernelInputViewModel> _dicById = new Dictionary<Guid, KernelInputViewModel>();

        private KernelInputViewModels() {
            Global.Access<KernelInputSetRefreshedEvent>(
                Guid.Parse("4027C417-0DEA-4C96-8794-A79D6602684D"),
                "内核输入数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                });
            Global.Access<KernelInputAddedEvent>(
                Guid.Parse("7BB2CAD5-333F-4BDD-B6FF-3F0AA50724EA"),
                "添加了内核输入后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    var vm = new KernelInputViewModel(message.Source);
                    _dicById.Add(message.Source.GetId(), vm);
                    OnPropertyChangeds();
                });
            Global.Access<KernelInputUpdatedEvent>(
                Guid.Parse("A85F4699-F884-43A3-B6F1-3E7CBCA7D7D6"),
                "更新了内核输入后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        var item = _dicById[message.Source.GetId()];
                        if (item != null) {
                            bool isSupportDualMine = item.IsSupportDualMine;
                            string args = item.Args;
                            string dualFullArgs = item.DualFullArgs;
                            item.Update(message.Source);
                            if (args != item.Args || dualFullArgs != item.DualFullArgs) {
                                CoinViewModel coinVm = MinerProfileViewModel.Current.CoinVm;
                                if (coinVm != null && coinVm.CoinKernel != null && coinVm.CoinKernel.Kernel.KernelInputId == item.Id) {
                                    Global.Execute(new RefreshArgsAssemblyCommand());
                                }
                            }
                            if (isSupportDualMine != item.IsSupportDualMine) {
                                foreach (var coinKernelVm in CoinKernelViewModels.Current.AllCoinKernels.Where(a => a.KernelId == message.Source.GetId())) {
                                    coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.IsSupportDualMine));
                                    coinKernelVm.OnPropertyChanged(nameof(coinKernelVm.DualCoinGroup));
                                }
                            }
                        }
                    }
                });
            Global.Access<KernelInputRemovedEvent>(
                Guid.Parse("4E0CFBAF-443F-4C09-B86B-3DBC7D7AF875"),
                "移除了内核输入后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChangeds();
                    }
                });
            Init();
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (var item in NTMinerRoot.Current.KernelInputSet) {
                    KernelInputViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        Global.Execute(new UpdateKernelInputCommand(item));
                    }
                    else {
                        Global.Execute(new AddKernelInputCommand(item));
                    }
                }
            }
            else {
                foreach (var item in NTMinerRoot.Current.KernelInputSet) {
                    _dicById.Add(item.GetId(), new KernelInputViewModel(item));
                }
            }
        }

        private void OnPropertyChangeds() {
            OnPropertyChanged(nameof(AllKernelInputVms));
            OnPropertyChanged(nameof(PleaseSelectVms));
        }

        public bool TryGetKernelInputVm(Guid id, out KernelInputViewModel kernelInputVm) {
            return _dicById.TryGetValue(id, out kernelInputVm);
        }

        public List<KernelInputViewModel> AllKernelInputVms {
            get {
                return _dicById.Values.OrderBy(a => a.Name).ToList();
            }
        }

        private IEnumerable<KernelInputViewModel> GetPleaseSelectVms() {
            yield return KernelInputViewModel.PleaseSelect;
            foreach (var item in _dicById.Values.OrderBy(a => a.Name)) {
                yield return item;
            }
        }

        public List<KernelInputViewModel> PleaseSelectVms {
            get {
                return GetPleaseSelectVms().ToList();
            }
        }
    }
}
