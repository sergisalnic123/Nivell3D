using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        private void Preprocess()
        {
            ProcessVegetationCellList.Clear();
            ProcessVegetationCellList.AddRange(VisibleVegetationCellList);
            ProcessVegetationCellList.AddRange(ShadowVegetationCellList);

            if (UseMultithreading && Application.isPlaying)
            {
                _needLoadVegetationList.Clear();

                for (int i = 0; i <= ProcessVegetationCellList.Count - 1; i++)
                {
                    if (ProcessVegetationCellList[i].NeedsLoadVegetation())
                        _needLoadVegetationList.Add(ProcessVegetationCellList[i]);
                }
                Profiler.BeginSample("Preprocess");

                if (_needLoadVegetationList.Count < 2)
                {
                    Profiler.BeginSample("Single thread load vegetation");
                    for (int j = 0; j <= _needLoadVegetationList.Count - 1; j++)
                    {
                        _needLoadVegetationList[j].LoadVegetation();
                    }
                    Profiler.EndSample();
                }
                else
                {
                    //TUDO get split lists from pool
                    List<List<VegetationCell>> splitList = SplitCellList(_needLoadVegetationList, ThreadCount);
                    Profiler.BeginSample("Multi thread load vegetation ");
                    ManualResetEvent[] resetEvents = new ManualResetEvent[splitList.Count];
                    int listSize = (_needLoadVegetationList.Count / ThreadCount) + 1;

                    for (int j = 0; j <= splitList.Count - 1; j++)
                    {
                        resetEvents[j] = new ManualResetEvent(false);
                        try
                        {

                            int jj = j;
                            ThreadPool.QueueUserWorkItem(
                                (obj) =>
                                {
                                    for (int i = 0; i <= splitList[jj].Count - 1; i++)
                                    {
                                        _needLoadVegetationList[listSize * jj + i].LoadVegetation();
                                    }
                                    resetEvents[jj].Set();
                                }
                            );
                        }
                        catch (Exception)
                        {
                            resetEvents[j].Set();
                        }
                    }

                    WaitHandle.WaitAll(resetEvents);
                    Profiler.EndSample();
                }

                Profiler.EndSample();
            }
            else
            {
                Profiler.BeginSample("Spawn cells and preprocess");
                for (int i = 0; i <= ProcessVegetationCellList.Count - 1; i++)
                {
                    Profiler.BeginSample("Load vegetation");
                    ProcessVegetationCellList[i].LoadVegetation();
                    Profiler.EndSample();
                }
                Profiler.EndSample();
            }

            for (int i = 0; i <= ProcessVegetationCellList.Count - 1; i++)
            {
                ProcessVegetationCellList[i].PostProcess(UseComputeShaders);
            }
        }

        void UpdateVegetationInstanceLists()
        {
            Preprocess();
            Profiler.BeginSample("Create Render Lists");
            if (UseListMultithreading && Application.isPlaying)
            {
                if (EnableUnlimitedVegetationItems)
                {
                    int itemCount = VegetationModelInfoList.Count;
                    int batchCount = Mathf.CeilToInt((float) itemCount / 64);
                    for (int i = 0; i <= batchCount - 1; i++)
                    {
                        ResetManualResetEvents();
                        int currentItemCount = Mathf.Clamp(itemCount - (i * 64),0,64);
                        for (int j = 0; j <= currentItemCount - 1; j++)
                        {
                            try
                            {
                                _manualResetEvent[j].Reset();
                                int re = j + i * 64;
                                int re2 = j;
                                ThreadPool.QueueUserWorkItem(
                                    (obj) =>
                                    {
                                        PrepareVegetationSplitList(VegetationModelInfoList[re].SplitVegetationInstanceList, VegetationModelInfoList[re].MatrixListPool);

                                        List<Matrix4x4> newSplitList = VegetationModelInfoList[re].MatrixListPool.GetList();

                                        int maxProcessVegetationCellListCount = ProcessVegetationCellList.Count - 1;
                                        for (int k = 0; k <= maxProcessVegetationCellListCount; k++)
                                        {
                                            if (!ProcessVegetationCellList[k].IsVisible && ProcessVegetationCellList[k].IsTreeOrLargeObject(re) == false) continue;

                                            List<Matrix4x4> tempInstanceList = ProcessVegetationCellList[k].GetCurrentVegetationList(re);
                                            if (tempInstanceList == null) continue;
                                            if (newSplitList.Count + tempInstanceList.Count < VegetationModelInfoList[re].MaxListSize)
                                            {
                                                newSplitList.AddRange(tempInstanceList);
                                            }
                                            else
                                            {
                                            int maxListSize = VegetationModelInfoList[re].MaxListSize;
                                                int maxTempInstanceListCount = tempInstanceList.Count - 1;
                                                for (int l = 0; l <= maxTempInstanceListCount; l++)
                                                {
                                                    newSplitList.Add(tempInstanceList[l]);
                                                    if (newSplitList.Count == maxListSize)
                                                    {
                                                        VegetationModelInfoList[re].SplitVegetationInstanceList.Add(newSplitList);
                                                        newSplitList = VegetationModelInfoList[re].MatrixListPool.GetList();
                                                    }
                                                }
                                            }
                                        }
                                        VegetationModelInfoList[re].SplitVegetationInstanceList.Add(newSplitList);
                                        _manualResetEvent[re2].Set();
                                    });
                            }
                            catch (Exception)
                            {
                                _manualResetEvent[j].Set();
                            }
                        }
                        WaitHandle.WaitAll(_manualResetEvent);
                    }
                }
                else
                {
                    ResetManualResetEvents();
                    for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
                    {
                        try
                        {
                            //resetEvents[i] = new ManualResetEvent(false);
                            _manualResetEvent[i].Reset();
                            int re = i;
                            ThreadPool.QueueUserWorkItem(
                                (obj) =>
                                {
                                    PrepareVegetationSplitList(VegetationModelInfoList[re].SplitVegetationInstanceList, VegetationModelInfoList[re].MatrixListPool);

                                    List<Matrix4x4> newSplitList = VegetationModelInfoList[re].MatrixListPool.GetList();

                                    int maxProcessVegetationCellListCount = ProcessVegetationCellList.Count - 1;
                                    for (int j = 0; j <= maxProcessVegetationCellListCount; j++)
                                    {
                                        if (!ProcessVegetationCellList[j].IsVisible && ProcessVegetationCellList[j].IsTreeOrLargeObject(re) == false) continue;

                                        List<Matrix4x4> tempInstanceList = ProcessVegetationCellList[j].GetCurrentVegetationList(re);
                                        if (tempInstanceList == null) continue;
                                        if (newSplitList.Count + tempInstanceList.Count < VegetationModelInfoList[re].MaxListSize)
                                        {
                                            newSplitList.AddRange(tempInstanceList);
                                        }
                                        else
                                        {
                                        //TUDO change to AddRange GetRange
                                        int maxListSize = VegetationModelInfoList[re].MaxListSize;
                                            int maxTempInstanceListCount = tempInstanceList.Count - 1;
                                            for (int k = 0; k <= maxTempInstanceListCount; k++)
                                            {
                                                newSplitList.Add(tempInstanceList[k]);
                                                if (newSplitList.Count == maxListSize)
                                                {
                                                    VegetationModelInfoList[re].SplitVegetationInstanceList.Add(newSplitList);
                                                    newSplitList = VegetationModelInfoList[re].MatrixListPool.GetList();
                                                }
                                            }
                                        }
                                    }
                                    VegetationModelInfoList[re].SplitVegetationInstanceList.Add(newSplitList);
                                    _manualResetEvent[re].Set();
                                });
                        }
                        catch (Exception)
                        {
                            _manualResetEvent[i].Set();
                        }
                    }
                    WaitHandle.WaitAll(_manualResetEvent);
                }
            }
            else
            {
                for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
                {                   
                    if (VegetationModelInfoList[i].VegetationRenderType == VegetationRenderType.InstancedIndirect && Application.isPlaying)
                    {
                        continue;
                    }
                    
                    PrepareVegetationSplitList(VegetationModelInfoList[i].SplitVegetationInstanceList, VegetationModelInfoList[i].MatrixListPool);
                    List<Matrix4x4> newSplitList = VegetationModelInfoList[i].MatrixListPool.GetList();

                    //if (VegetationModelInfoList[i].VegetationRenderType == VegetationRenderType.InstancedIndirect && Application.isPlaying)
                    //{
                    //    VegetationModelInfoList[i].SplitVegetationInstanceList.Add(newSplitList);
                    //    continue;
                    //}

                    for (int j = 0; j <= ProcessVegetationCellList.Count - 1; j++)
                    {
                        if (!ProcessVegetationCellList[j].IsVisible && ProcessVegetationCellList[j].IsTreeOrLargeObject(i) == false) continue;

                        List<Matrix4x4> tempInstanceList = ProcessVegetationCellList[j].GetCurrentVegetationList(i);
                        if (tempInstanceList == null) continue;
                        if (newSplitList.Count + tempInstanceList.Count < VegetationModelInfoList[i].MaxListSize)
                        {
                            newSplitList.AddRange(tempInstanceList);
                        }
                        else
                        {
                            for (int k = 0; k <= tempInstanceList.Count - 1; k++)
                            {
                                newSplitList.Add(tempInstanceList[k]);
                                if (newSplitList.Count == VegetationModelInfoList[i].MaxListSize)
                                {
                                    VegetationModelInfoList[i].SplitVegetationInstanceList.Add(newSplitList);
                                    newSplitList = VegetationModelInfoList[i].MatrixListPool.GetList();
                                }
                            }
                        }
                    }
                    VegetationModelInfoList[i].SplitVegetationInstanceList.Add(newSplitList);
                }
            }

            Profiler.EndSample();
        }
    }
}