﻿using System;
using System.Collections.Generic;

namespace NTMiner.Core.Kernels {
    public static class KernelSetExtension {
        /// <summary>
        /// 获取NTMiner所支持的所有内核的近处名
        /// </summary>
        /// <param name="kernelSet"></param>
        /// <returns></returns>
        public static HashSet<string> GetAllKernelProcessNames(this IKernelSet kernelSet) {
#if DEBUG
            NTStopwatch.Start();
#endif
            HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kernel in kernelSet.AsEnumerable()) {
                hashSet.Add(kernel.GetProcessName());
            }
#if DEBUG
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {nameof(KernelSetExtension)}.{nameof(GetAllKernelProcessNames)}()");
            }
#endif
            return hashSet;
        }
    }
}
