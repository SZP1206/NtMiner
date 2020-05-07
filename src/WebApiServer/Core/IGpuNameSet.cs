﻿using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IGpuNameSet {
        void AddCount(GpuType gpuType, string gpuName, ulong gpuTotalMemory);
        IEnumerable<GpuNameCount> GetGpuNameCounts();
        /// <summary>
        /// 注意：整个GpuName是个值对象，且GpuName作为key存储为hash表，所以不支持Update
        /// </summary>
        /// <param name="gpuName"></param>
        void Set(GpuName gpuName);
        void Remove(GpuName gpuName);
        IEnumerable<GpuName> AsEnumerable();
    }
}