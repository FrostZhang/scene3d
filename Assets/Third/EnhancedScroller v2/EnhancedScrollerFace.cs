using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(EnhancedScroller))]
public class EnhancedScrollerFace : MonoBehaviour, IEnhancedScrollerDelegate
{
    [HideInInspector]
    public EnhancedScroller scroller;
    [SerializeField] EnhancedScrollerCellView cellViewPrefab;
    public Action<EnhancedScrollerCellView, int, int> onShowCell;
    public int maxCount = 10;
    public int cellsize = 50;

    void Awake()
    {
        scroller = GetComponent<EnhancedScroller>();
        scroller.Delegate = this;
        if (!cellViewPrefab)
            cellViewPrefab = GetComponentInChildren<EnhancedScrollerCellView>();
        if (cellViewPrefab)
            cellViewPrefab.gameObject.SetActive(false);
        else
            throw new NullReferenceException("无限循环列表没有prefab!");
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cell = scroller.GetCellView(cellViewPrefab);
        onShowCell?.Invoke(cell, dataIndex, cellIndex);
        return cell;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellsize;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return maxCount;
    }

}
