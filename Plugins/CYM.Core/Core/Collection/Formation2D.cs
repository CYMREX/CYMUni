//------------------------------------------------------------------------------
// Array2D.cs
// Copyright 2019 2019/12/27 
// Created by CYM on 2019/12/27
// Owner: CYM
// 填写类的描述...
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CYM
{
    [Unobfus]
    public class Formation2D<T> where T : class
    {
        public int CurIndex { get; set; } = 0;
        public T[,] Map;
        public Dictionary<T, Tuple<int, int>> ValuePos = null;
        public List<T> Data;
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Formation2D(int row, int col)
        {
            Map = new T[row, col];
            ValuePos = new Dictionary<T, Tuple<int, int>>();
            Data = new List<T>();
            Row = row;
            Col = col;
        }

        #region set
        public void Place(T data)
        {
            var pos = GetPos(CurIndex);
            Place(data, pos.Item1, pos.Item2);
            CurIndex++;
        }
        public void Place(T data, int x, int y)
        {
            if (!IsValid(x, y))
            {
                Debug.LogError(string.Format("无效的坐标,{0}:{1}", x, y));
                return;
            }
            Remove(data);
            Remove(x, y);
            if (!ValuePos.ContainsKey(data)) ValuePos.Add(data, new Tuple<int, int>(x, y));
            else Debug.LogError("重复的Key");
            Map[x, y] = data;
            Data.Add(data);
        }
        public void Remove(T data)
        {
            if (ValuePos.ContainsKey(data))
            {
                Tuple<int, int> pos = ValuePos[data];
                if (!IsValid(pos.Item1, pos.Item2))
                {
                    Debug.LogError(string.Format("无效的坐标,{0}:{1}", pos.Item1, pos.Item2));
                    return;
                }
                ValuePos.Remove(data);
                Map[pos.Item1, pos.Item2] = default;
                Data.Remove(data);
            }
        }
        public void Remove(int x, int y)
        {
            if (!IsValid(x, y))
            {
                Debug.LogError(string.Format("无效的坐标,{0}:{1}", x, y));
                return;
            }
            var data = Map[x, y];
            T t = default(T);
            if (data != t)
            {
                ValuePos.Remove(data);
                Data.Remove(data);
                Map[x, y] = t;
            }
        }
        public void Clear()
        {
            CurIndex = 0;
            Map = new T[Row, Col];
            ValuePos.Clear();
            Data.Clear();
        }
        #endregion

        #region get
        public Tuple<int, int> GetPos(int index)
        {
            if (index >= Map.Length)
            {
                throw new Exception("错误的索引:" + index);
            }
            return new Tuple<int, int>(GetRow(index), GetCol(index));

        }
        public Tuple<int, int> GetPos(T data)
        {
            if (!ValuePos.ContainsKey(data))
            {
                throw new Exception("不包含:" + data.ToString());
            }
            return ValuePos[data];
        }
        public int GetRow(int index) => index % Row;
        public int GetCol(int index) => Mathf.CeilToInt(index / Row);
        //public T GetNearest(int x,int y)
        //{ 

        //}
        #endregion

        #region is
        public bool IsValid(int x, int y)
        {
            if (x >= Row) return false;
            else if (y >= Col) return false;
            return true;
        }
        #endregion
    }
}