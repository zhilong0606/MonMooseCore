using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    [AddComponentMenu("Layout/UIGrid", 152)]
    public class UIGrid : LayoutGroup
    {
        public enum Corner
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight,
        }

        public enum ScrollEndAnchor
        {
            Auto,
            Center,
            UpperOrRight,
            LowerOrLeft,
        }

        public enum EmptyFillType
        {
            None,
            Line,
            Viewport,
        }

        public enum SelectType
        {
            None,
            One,
            OneJust,
            More,
            MoreLimited,
        }

        public enum AutoCollectStaticType
        {
            None,
            Head,
            Tail,
        }

        public enum EIndexPart
        {
            None,
            StaticHead,
            Dynamic,
            StaticTail,
            Empty,
        }

        private class DummyInfo
        {
        }

        [SerializeField] protected RectTransform.Axis m_axis = RectTransform.Axis.Horizontal;
        [SerializeField] protected Corner m_startCorner = Corner.UpperLeft;
        [SerializeField] protected RectTransform.Axis m_startAxis = RectTransform.Axis.Horizontal;
        [SerializeField] protected Vector2 m_cellSize = new Vector2(100, 100);
        [SerializeField] protected Vector2 m_spacing = Vector2.zero;
        [SerializeField] protected int m_constraintCount = 1;
        [SerializeField] protected ScrollRect m_scrollRect;
        [SerializeField] protected GameObject m_dynamicCell;
        [SerializeField] protected int m_dynamicCellCapacity;
        [SerializeField] protected List<GameObject> m_headStaticObjList = new List<GameObject>();
        [SerializeField] protected List<GameObject> m_tailStaticObjList = new List<GameObject>();
        [SerializeField] protected bool m_isStaticInRule = true;
        [SerializeField] private AutoCollectStaticType m_autoCollectType = AutoCollectStaticType.Head;
        [SerializeField] protected EmptyFillType m_emptyFillType = EmptyFillType.None;
        [SerializeField] protected GameObject m_emptyCell;
        [SerializeField] protected int m_emptyCellCapacity;
        [SerializeField] protected int m_lazyCount = 1;
        [SerializeField] protected bool m_disableScrollWhenLessCell = false;
        [SerializeField] protected List<GameObject> m_ignoreList = new List<GameObject>();
        [SerializeField] protected SelectType m_selectType = SelectType.None;
        [SerializeField] protected bool m_allowSelectSameAgain = false;
        [SerializeField] protected int m_selectLimitedCount = 0;

        public event Action eventOnSelectedListChanged;
        public event CellSelectChangedDelegate eventOnSelectedStateChanged;
        public event CellEventHappenedDelegate eventOnCellEventHappened;

        private int m_cellsPerMainAxis;
        private int m_actualCellCountX;
        private int m_actualCellCountY;
        private Vector2 m_startOffset;
        private float m_triggerPosMin;
        private float m_triggerPosMax;
        private int m_visiblePosMin;
        private int m_visiblePosMax;
        private List<UICell> m_cellList = new List<UICell>();
        private List<UICell> m_emptyCellList = new List<UICell>();
        private List<UICell> m_headStaticCellList = new List<UICell>();
        private List<UICell> m_tailStaticCellList = new List<UICell>();
        private List<object> m_infoList = new List<object>();
        private int m_infoCount;
        private Func<GameObject, UICell> m_funcOnInitCell;
        private Func<GameObject, UICell> m_funcOnInitEmptyCell;
        private Func<int, int, object> m_funcOnUpdateEmptyCell;
        private Vector2 m_targetLerpPos = Vector2.zero;
        private Vector2 m_startLerpPos = Vector2.zero;
        private bool m_startLerp = false;
        private float m_lerpTime;
        private float m_curTime;
        private Action m_actionOnScrollEnd;
        private List<object> m_selectedInfoList = new List<object>();
        private bool m_isInitialized = false;
        private Stack<DummyInfo> m_dummyInfoStack = new Stack<DummyInfo>();
        private List<DummyInfo> m_dummyInfoList = new List<DummyInfo>();
        private List<UICell> m_hideCellList = new List<UICell>();
        private bool m_needUpdateGridPlaying = false;

        public Corner startCorner
        {
            get { return m_startCorner; }
            set { SetProperty(ref m_startCorner, value); }
        }

        public Vector2 cellSize
        {
            get { return m_cellSize; }
            set { SetProperty(ref m_cellSize, value); }
        }

        public Vector2 spacing
        {
            get { return m_spacing; }
            set { SetProperty(ref m_spacing, value); }
        }

        public int constraintCount
        {
            get { return m_constraintCount; }
            set { SetProperty(ref m_constraintCount, Mathf.Max(1, value)); }
        }

        public ScrollRect scrollRect
        {
            get { return m_scrollRect; }
            set { SetProperty(ref m_scrollRect, value); }
        }

        public int selectedIndex
        {
            get { return m_infoList.IndexOf(selectedInfo); }
        }

        public int selectLimitedCount
        {
            get { return m_selectLimitedCount; }
        }

        public int cellCount
        {
            get
            {
                if (m_isStaticInRule)
                {
                    return m_cellList.Count + GetHeadStaticCellCount() + GetTailStaticCellCount();
                }
                return m_cellList.Count;
            }
        }

        public object selectedInfo
        {
            get
            {
                if (m_selectedInfoList.Count > 0)
                {
                    return m_selectedInfoList[0];
                }
                return null;
            }
        }

        public List<object> selectedInfoList
        {
            get { return m_selectedInfoList; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            constraintCount = constraintCount;
            if (scrollRect == null)
            {
                scrollRect = GetScrollRect();
            }
            SetDirty();
        }
#endif
        public void Initialize(Func<GameObject, UICell> funcOnInitCell)
        {
            Initialize(funcOnInitCell, null, null);
        }

        public void Initialize(Func<GameObject, UICell> funcOnInitCell, Func<GameObject, UICell> funcOnInitEmptyCell, Func<int, int, object> funcOnUpdateEmptyCell)
        {
            if (m_isInitialized)
            {
                return;
            }
            m_isInitialized = true;
            m_funcOnInitCell = funcOnInitCell;
            m_funcOnInitEmptyCell = funcOnInitEmptyCell;
            m_funcOnUpdateEmptyCell = funcOnUpdateEmptyCell;
            if (scrollRect == null)
            {
                scrollRect = GetScrollRect();
            }
            if (CheckInScroll(false))
            {
                scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            }
            InitDynamicCells();
            InitEmptyCells();
            InitStaticCellList(m_headStaticObjList, m_headStaticCellList, AutoCollectStaticType.Head);
            InitStaticCellList(m_tailStaticObjList, m_tailStaticCellList, AutoCollectStaticType.Tail);
            HideUnknownChild();
        }

        protected void InitDynamicCells()
        {
            if (m_dynamicCell != null && !gameObject.IsParentOf(m_dynamicCell))
            {
                m_dynamicCell = GameObject.Instantiate(m_dynamicCell, transform);
            }
            m_dynamicCell.SetActiveSafely(true);
            for (int i = 0; i < m_dynamicCellCapacity; ++i)
            {
                RecycleCell(InstantiateCell());
            }
            m_dynamicCell.SetActiveSafely(false);
        }

        protected void InitEmptyCells()
        {
            for (int i = 0; i < m_emptyCellCapacity; ++i)
            {
                RecycleCell(InstantiateEmptyCell());
            }
            if (m_emptyCell != null && m_emptyCell.activeSelf)
            {
                m_emptyCell.SetActive(false);
            }
        }

        private bool IsUnknownChild(GameObject go)
        {
            return !(go == m_dynamicCell
                     || go == m_emptyCell
                     || m_ignoreList.Contains(go)
                     || m_headStaticObjList.Contains(go)
                     || m_tailStaticObjList.Contains(go));
        }

        private void InitStaticCellList(List<GameObject> objList, List<UICell> cellList, AutoCollectStaticType collectType)
        {
            if (collectType == m_autoCollectType)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    GameObject childObj = transform.GetChild(i).gameObject;
                    if (IsUnknownChild(childObj))
                    {
                        objList.Add(childObj);
                    }
                }
            }
            if (m_isStaticInRule && m_funcOnInitCell != null)
            {
                for (int i = 0; i < objList.Count; ++i)
                {
                    UICell cell = m_funcOnInitCell(objList[i]);
                    BindCell(cell);
                    cellList.Add(cell);
                }
            }
        }

        private void HideUnknownChild()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject childObj = transform.GetChild(i).gameObject;
                if (IsUnknownChild(childObj) && childObj.activeSelf)
                {
                    childObj.SetActive(false);
                }
            }
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int minColumns = 0;
            int preferredColumns = 0;
            RectTransform.Axis axis = GetAxis();
            if (axis == RectTransform.Axis.Horizontal)
            {
                minColumns = preferredColumns = Mathf.CeilToInt(GetChildCount() / (float)m_constraintCount - 0.001f);
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                minColumns = preferredColumns = m_constraintCount;
            }
            float totalMin = padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x;
            float totalPreferred = padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x;
            float totalFlexible = -1f;
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, (int)RectTransform.Axis.Horizontal);
        }

        public override void SetLayoutHorizontal()
        {
            UpdateContentSize(RectTransform.Axis.Horizontal);
        }

        public override void CalculateLayoutInputVertical()
        {
            int minRows = 0;
            RectTransform.Axis axis = GetAxis();
            if (axis == RectTransform.Axis.Horizontal)
            {
                minRows = m_constraintCount;
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                minRows = Mathf.CeilToInt(GetChildCount() / (float)m_constraintCount - 0.001f);
            }
            float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
            float totalFlexible = -1f;
            SetLayoutInputForAxis(minSpace, minSpace, totalFlexible, (int)RectTransform.Axis.Vertical);
        }

        public override void SetLayoutVertical()
        {
            UpdateContentSize(RectTransform.Axis.Vertical);
            UpdateGrid(false);
        }

        public void RebuildImmediately()
        {
            CalculateLayoutInputHorizontal();
            SetLayoutHorizontal();
            CalculateLayoutInputVertical();
            UpdateContentSize(RectTransform.Axis.Vertical);
            UpdateGrid(true);
        }

        private void UpdateGrid(bool immediately)
        {
            UpdateGridFieldData();
            if (m_isInitialized)
            {
                if (immediately)
                {
                    UpdateGridPlaying();
                }
                else
                {
                    m_needUpdateGridPlaying = true;
                }
            }
            else
            {
                UpdateGridPreview();
            }
        }

        private void UpdateContentSize(RectTransform.Axis axis)
        {
            if (axis != GetAxis())
            {
                return;
            }
            if (CheckInScroll(false))
            {
                rectTransform.SetSizeWithCurrentAnchors(axis, LayoutUtility.GetPreferredSize(rectTransform, (int)axis));
            }
        }

        private void UpdateGridFieldData()
        {
            int cellCountX = 1;
            int cellCountY = 1;
            RectTransform.Axis axis = GetAxis();
            int childCount = GetChildCount();
            if (axis == RectTransform.Axis.Horizontal)
            {
                cellCountY = m_constraintCount;
                cellCountX = Mathf.CeilToInt(childCount / (float)cellCountY - 0.001f);
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                cellCountX = m_constraintCount;
                cellCountY = Mathf.CeilToInt(childCount / (float)cellCountX - 0.001f);
            }
            if (m_startAxis == RectTransform.Axis.Horizontal)
            {
                m_cellsPerMainAxis = cellCountX;
                m_actualCellCountX = Mathf.Clamp(cellCountX, 1, childCount);
                m_actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(childCount / (float)m_cellsPerMainAxis));
            }
            else if (m_startAxis == RectTransform.Axis.Vertical)
            {
                m_cellsPerMainAxis = cellCountY;
                m_actualCellCountY = Mathf.Clamp(cellCountY, 1, childCount);
                m_actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(childCount / (float)m_cellsPerMainAxis));
            }
            Vector2 requiredSpace = new Vector2(
                m_actualCellCountX * cellSize.x + (m_actualCellCountX - 1) * spacing.x,
                m_actualCellCountY * cellSize.y + (m_actualCellCountY - 1) * spacing.y
            );
            m_startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
            );
        }

        private void UpdateGridPlaying()
        {
            UpdateVisibleRange();
            RectTransform.Axis axis = GetAxis();
            int headStaticCellIndex = 0;
            int cellIndex = 0;
            int tailStaticCellIndex = 0;
            int emptyCellIndex = 0;
            for (int i = 0; i < GetChildCount(); i++)
            {
                int positionX;
                int positionY;
                GetPosByIndex(i, out positionX, out positionY);
                if (IsIndexNeedCheckSkip(i) && !IsIndexVisible(axis, positionX, positionY))
                {
                    continue;
                }
                object info = null;
                UICell cell = null;
                RectTransform childTrans = null;
                EIndexPart indexPart = GetIndexPart(i);
                if (indexPart == EIndexPart.StaticHead)
                {
                    if (m_isStaticInRule)
                    {
                        info = m_infoList[i];
                        cell = m_headStaticCellList[headStaticCellIndex];
                    }
                    else
                    {
                        childTrans = m_headStaticObjList[headStaticCellIndex].transform as RectTransform;
                    }
                    headStaticCellIndex++;
                }
                else if (indexPart == EIndexPart.StaticTail)
                {
                    if (m_isStaticInRule)
                    {
                        info = m_infoList[i];
                        cell = m_tailStaticCellList[tailStaticCellIndex];
                    }
                    else
                    {
                        childTrans = m_tailStaticObjList[tailStaticCellIndex].transform as RectTransform;
                    }
                    tailStaticCellIndex++;
                }
                else if (indexPart == EIndexPart.Dynamic)
                {
                    info = m_infoList[m_isStaticInRule ? i : i - GetHeadStaticCellCount()];
                    cell = cellIndex < m_cellList.Count ? m_cellList[cellIndex] : InstantiateCell();
                    RenovateCell(cell);
                    cellIndex++;
                }
                else if (indexPart == EIndexPart.Empty)
                {
                    info = m_funcOnUpdateEmptyCell == null ? null : m_funcOnUpdateEmptyCell(positionX, positionY);
                    cell = emptyCellIndex < m_emptyCellList.Count ? m_emptyCellList[emptyCellIndex] : InstantiateEmptyCell();
                    RenovateCell(cell);
                    emptyCellIndex++;
                }
                if (cell != null)
                {
                    childTrans = cell.transform as RectTransform;
                    cell.index = i;
                    int infoIndex = i;
                    if (!m_isStaticInRule)
                    {
                        infoIndex -= GetHeadStaticCellCount();
                    }
                    cell.UpdateCell(m_infoList[infoIndex]);
                    IUICellSelectable selectable = cell as IUICellSelectable;
                    if (selectable != null)
                    {
                        selectable.IsSelected = m_selectedInfoList.Contains(m_infoList[infoIndex]);
                    }
                }
                if (childTrans != null)
                {
                    SetChildAlongAxis(childTrans, 0, m_startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
                    SetChildAlongAxis(childTrans, 1, m_startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
                }
            }
            for (int i = m_cellList.Count - 1; i >= cellIndex; --i)
            {
                RecycleCell(m_cellList[i]);
            }
            for (int i = m_emptyCellList.Count - 1; i >= emptyCellIndex; --i)
            {
                RecycleCell(m_emptyCellList[i]);
            }
            UpdateHideCells();
        }

        private void UpdateGridPreview()
        {
            for (int i = 0; i < rectChildren.Count; i++)
            {
                int positionX;
                int positionY;
                GetPosByIndex(i, out positionX, out positionY);
                if (!m_ignoreList.Contains(rectChildren[i].gameObject))
                {
                    SetChildAlongAxis(rectChildren[i], 0, m_startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
                    SetChildAlongAxis(rectChildren[i], 1, m_startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
                }
            }
        }

        private void OnScrollRectValueChanged(Vector2 value)
        {
            RectTransform.Axis axis = GetAxis();
            Rect cutRect = GetCutRect();
            if (axis == RectTransform.Axis.Horizontal)
            {
                if (cutRect.xMax >= m_triggerPosMax || cutRect.xMin <= m_triggerPosMin)
                {
                    UpdateGridPlaying();
                }
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                if (cutRect.yMax >= m_triggerPosMax || cutRect.yMin <= m_triggerPosMin)
                {
                    UpdateGridPlaying();
                }
            }
        }

        private void OnCellEventHappened(int eventId, int index, object obj)
        {
            if (eventOnCellEventHappened != null)
            {
                eventOnCellEventHappened(eventId, index, obj);
            }
        }

        private ScrollRect GetScrollRect()
        {
            ScrollRect sr = GetComponentInParent<ScrollRect>();
            if (sr != null && sr.content == transform)
            {
                return sr;
            }
            return null;
        }

        private RectTransform.Axis GetAxis()
        {
            if (CheckInScroll(false))
            {
                return m_scrollRect.horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
            }
            return m_axis;
        }

        private Rect GetViewportRect()
        {
            if (m_scrollRect != null && m_scrollRect.viewport != null)
            {
                return m_scrollRect.viewport.rect;
            }
            return default(Rect);
        }

        public int GetChildCount(bool ignoreEmpty = false)
        {
            int count = 0;
            if (!m_isInitialized)
            {
                for (int i = 0; i < rectChildren.Count; ++i)
                {
                    if (!m_ignoreList.Contains(rectChildren[i].gameObject))
                    {
                        count++;
                    }
                }
            }
            else
            {
                count = m_infoCount;
                if (!ignoreEmpty)
                {
                    count += GetEmptyCount(m_emptyFillType);
                }
                if (!m_isStaticInRule)
                {
                    count += m_headStaticObjList.Count + m_tailStaticObjList.Count;
                }
            }
            return count;
        }

        private bool IsIndexNeedCheckSkip(int index)
        {
            if (CheckInScroll(false))
            {
                if (m_isStaticInRule)
                {
                    return index >= GetHeadStaticCellCount() && index < m_infoCount - GetTailStaticCellCount();
                }
                else
                {
                    return index >= GetHeadStaticCellCount() && index < GetHeadStaticCellCount() + m_infoCount;
                }
            }
            return false;
        }

        private bool IsIndexVisible(RectTransform.Axis axis, int positionX, int positionY)
        {
            if (axis == RectTransform.Axis.Horizontal)
            {
                return positionX >= m_visiblePosMin && positionX <= m_visiblePosMax;
            }
            if (axis == RectTransform.Axis.Vertical)
            {
                return positionY >= m_visiblePosMin && positionY <= m_visiblePosMax;
            }
            return true;
        }

        private EIndexPart GetIndexPart(int index)
        {
            if (index < GetHeadStaticCellCount())
            {
                return EIndexPart.StaticHead;
            }
            int dynamicCount = m_infoCount;
            if (m_isStaticInRule)
            {
                dynamicCount -= GetHeadStaticCellCount() + GetTailStaticCellCount();
            }
            if (index < GetHeadStaticCellCount() + dynamicCount)
            {
                return EIndexPart.Dynamic;
            }
            if (index < GetHeadStaticCellCount() + dynamicCount + GetTailStaticCellCount())
            {
                return EIndexPart.StaticTail;
            }
            return EIndexPart.Empty;
        }

        private void UpdateVisibleRange()
        {
            if (!CheckInScroll(false))
            {
                return;
            }
            RectTransform.Axis axis = GetAxis();
            Rect contentRect = rectTransform.rect;
            Rect cutRect = GetCutRect();
            if (axis == RectTransform.Axis.Horizontal)
            {
                m_visiblePosMin = (int)((cutRect.xMin - contentRect.xMin - m_Padding.left) / (m_spacing.x + m_cellSize.x)) - m_lazyCount;
                m_visiblePosMax = m_visiblePosMin + Mathf.CeilToInt(cutRect.width / (m_cellSize.x + m_spacing.x)) + m_lazyCount * 2;
                m_triggerPosMin = m_visiblePosMin <= 0 ? float.MinValue : contentRect.xMin + m_Padding.left + (m_cellSize.x + m_spacing.x) * m_visiblePosMin;
                m_triggerPosMax = m_visiblePosMax >= m_actualCellCountX - 1 ? float.MaxValue : contentRect.xMin + m_Padding.left + (m_cellSize.x + m_spacing.x) * (m_visiblePosMax + 1);
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                m_visiblePosMin = (int)((contentRect.yMax - m_Padding.top - cutRect.yMax) / (m_spacing.y + m_cellSize.y)) - m_lazyCount;
                m_visiblePosMax = m_visiblePosMin + Mathf.CeilToInt(cutRect.height / (m_cellSize.y + m_spacing.y)) + m_lazyCount * 2;
                m_triggerPosMax = m_visiblePosMin <= 0 ? float.MaxValue : contentRect.yMax - m_Padding.top - (m_cellSize.y + m_spacing.y) * m_visiblePosMin;
                m_triggerPosMin = m_visiblePosMax >= m_actualCellCountY - 1 ? float.MinValue : contentRect.yMax - m_Padding.top - (m_cellSize.y + m_spacing.y) * (m_visiblePosMax + 1);
            }
        }

        private void GetPosByIndex(int index, out int positionX, out int positionY)
        {
            positionX = 0;
            positionY = 0;
            if (m_startAxis == RectTransform.Axis.Horizontal)
            {
                positionX = index % m_cellsPerMainAxis;
                positionY = index / m_cellsPerMainAxis;
            }
            else if (m_startAxis == RectTransform.Axis.Vertical)
            {
                positionX = index / m_cellsPerMainAxis;
                positionY = index % m_cellsPerMainAxis;
            }
            if (m_startCorner == Corner.UpperRight || m_startCorner == Corner.LowerRight)
            {
                positionX = m_actualCellCountX - 1 - positionX;
            }
            if (m_startCorner == Corner.LowerLeft || m_startCorner == Corner.LowerRight)
            {
                positionY = m_actualCellCountY - 1 - positionY;
            }
        }

        private bool CheckInScroll(bool needLogOut)
        {
            if (m_scrollRect != null)
            {
                return true;
            }
            if (needLogOut)
            {
                Debug.LogError(string.Format("Error: {0} has UIGrid, But not in ScrollRect!!!", gameObject.name));
            }
            return false;
        }

        private Rect GetCutRect()
        {
            if (m_scrollRect == null || m_scrollRect.viewport == null || m_scrollRect.content == null)
            {
                return default(Rect);
            }
            Rect viewportRect = GetViewportRect();
            Rect contentRect = rectTransform.rect;
            Vector2 pos = Vector2.zero;
            Vector2 size = Vector2.zero;
            for (int i = 0; i < 2; ++i)
            {
                if (contentRect.size[i] > viewportRect.size[i])
                {
                    pos[i] = contentRect.center[i] - contentRect.size[i] / 2f + (contentRect.size[i] - viewportRect.size[i]) * m_scrollRect.normalizedPosition[i];
                    size[i] = viewportRect.size[i];
                }
                else
                {
                    pos[i] = contentRect.center[i] - contentRect.size[i] / 2f;
                    size[i] = contentRect.size[i];
                }
            }
            return new Rect(pos.x, pos.y, size.x, size.y);
        }

        public int GetHeadStaticCellCount()
        {
            return m_headStaticObjList.Count;
        }

        public int GetTailStaticCellCount()
        {
            return m_tailStaticObjList.Count;
        }

        public int GetEmptyCount(EmptyFillType emptyFillType)
        {
            int emptyCellCount = 0;
            if (emptyFillType != EmptyFillType.None)
            {
                emptyCellCount = m_infoCount % constraintCount == 0 ? 0 : constraintCount - m_infoCount % constraintCount;
                if (emptyFillType == EmptyFillType.Viewport)
                {
                    int filledLineCount = Mathf.CeilToInt((float)m_infoCount / constraintCount);
                    int viewportLineCount = GetViewportLineCount();
                    emptyCellCount += Mathf.Max(0, viewportLineCount - filledLineCount) * constraintCount;
                }
            }
            return emptyCellCount;
        }

        private int GetViewportLineCount()
        {
            RectTransform.Axis axis = GetAxis();
            if (axis == RectTransform.Axis.Horizontal)
            {
                return (int)((GetViewportRect().width - m_Padding.left - m_Padding.right) / (m_cellSize.x + m_spacing.x));
            }
            if (axis == RectTransform.Axis.Vertical)
            {
                return (int)((GetViewportRect().height - m_Padding.top - m_Padding.bottom) / (m_cellSize.y + m_spacing.y));
            }
            return 0;
        }

        private Vector2 GetDefaultNormalizedPosition()
        {
            Vector2 pos = scrollRect.normalizedPosition;
            RectTransform.Axis axis = GetAxis();
            pos[(int)axis] = Mathf.Clamp01((rectTransform.anchorMin[(int)axis] + rectTransform.anchorMax[(int)axis]) / 2f);
            return pos;
        }

        private ScrollEndAnchor GetAutoAnchor()
        {
            RectTransform.Axis axis = GetAxis();
            if (rectTransform.anchorMin[(int)axis] <= 0f && rectTransform.anchorMax[(int)axis] <= 0f)
            {
                return ScrollEndAnchor.LowerOrLeft;
            }
            if (rectTransform.anchorMin[(int)axis] >= 1f && rectTransform.anchorMax[(int)axis] >= 1f)
            {
                return ScrollEndAnchor.UpperOrRight;
            }
            return ScrollEndAnchor.Center;
        }

        private UICell InstantiateCell()
        {
            GameObject go = Instantiate(m_dynamicCell, transform);
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            UICell cell = null;
            if (m_funcOnInitCell != null)
            {
                cell = m_funcOnInitCell(go);
            }
            if (cell != null)
            {
                BindCell(cell);
                m_cellList.Add(cell);
            }
            return cell;
        }

        private void BindCell(UICell cell)
        {
            cell.ownerGrid = this;
            cell.eventOnCellEventHappened += OnCellEventHappened;
        }

        private void RenovateCell(UICell cell)
        {
            cell.Renovate();
            m_hideCellList.Remove(cell);
        }

        private void RecycleCell(UICell cell)
        {
            cell.Recycle();
            if (!m_hideCellList.Contains(cell))
            {
                m_hideCellList.Add(cell);
            }
        }

        private void UpdateHideCells()
        {
            RectTransform.Axis axis = GetAxis();
            int offsetLine = GetViewportLineCount() * 1000;
            for (int i = 0; i < m_hideCellList.Count; ++i)
            {
                int positionX;
                int positionY;
                RectTransform childTrans = m_hideCellList[i].transform as RectTransform;
                GetPosByIndex(i, out positionX, out positionY);
                if (axis == RectTransform.Axis.Horizontal)
                {
                    positionX += m_actualCellCountX + offsetLine;
                }
                else if (axis == RectTransform.Axis.Vertical)
                {
                    positionY += m_actualCellCountY + offsetLine;
                }
                SetChildAlongAxis(childTrans, 0, m_startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
                SetChildAlongAxis(childTrans, 1, m_startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);
            }
        }

        private UICell InstantiateEmptyCell()
        {
            GameObject go = Instantiate(m_emptyCell, transform);
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            UICell cell = null;
            if (m_funcOnInitEmptyCell != null)
            {
                cell = m_funcOnInitEmptyCell(go);
            }
            if (cell != null)
            {
                m_emptyCellList.Add(cell);
            }
            return cell;
        }

        private void ClearDummyInfoList()
        {
            for (int i = 0; i < m_dummyInfoList.Count; ++i)
            {
                m_dummyInfoStack.Push(m_dummyInfoList[i]);
            }
            m_dummyInfoList.Clear();
        }

        private DummyInfo NewDummyInfo()
        {
            DummyInfo info = null;
            if (m_dummyInfoStack.Count > 0)
            {
                info = m_dummyInfoStack.Pop();
            }
            else
            {
                info = new DummyInfo();
            }
            return info;
        }

        private bool SetCellListSelectState(List<UICell> list, object info, bool flag)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].info == info)
                {
                    IUICellSelectable selectable = list[i] as IUICellSelectable;
                    if (selectable != null)
                    {
                        selectable.IsSelected = flag;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool SetSelectState(object info, bool flag)
        {
            if (m_isStaticInRule)
            {
                if (SetCellListSelectState(m_headStaticCellList, info, flag)
                    || SetCellListSelectState(m_cellList, info, flag)
                    || SetCellListSelectState(m_tailStaticCellList, info, flag)) ;
            }
            else
            {
                SetCellListSelectState(m_cellList, info, flag);
            }
            bool result = false;
            if (flag)
            {
                if (!m_selectedInfoList.Contains(info))
                {
                    m_selectedInfoList.Add(info);
                    result = true;
                }
            }
            else
            {
                if (m_selectedInfoList.Contains(info))
                {
                    m_selectedInfoList.Remove(info);
                    result = true;
                }
            }
            if (result)
            {
                if (eventOnSelectedStateChanged != null)
                {
                    eventOnSelectedStateChanged(m_infoList.IndexOf(info), flag);
                }
            }
            return result;
        }

        private void Update()
        {
            if (m_needUpdateGridPlaying)
            {
                UpdateGridPlaying();
                m_needUpdateGridPlaying = false;
            }
            if (m_startLerp)
            {
                m_curTime += Time.deltaTime;
                float f = m_curTime / m_lerpTime;
                f = (Mathf.Sin(Mathf.PI * f - Mathf.PI * 0.5f) + 1f) / 2f;
                m_scrollRect.normalizedPosition = Vector2.Lerp(m_startLerpPos, m_targetLerpPos, f);
                if (m_curTime > m_lerpTime)
                {
                    ScrollEnd();
                }
            }
        }

        public void Refresh()
        {
            Refresh(false);
        }

        public void Refresh(bool resetSelect)
        {
            int count = 0;
            if (m_isStaticInRule)
            {
                count = GetHeadStaticCellCount() + GetTailStaticCellCount();
            }
            Refresh(count, resetSelect);
        }

        public void Refresh(int count)
        {
            Refresh(count, false);
        }

        public void Refresh(int count, bool resetSelect)
        {
            ClearDummyInfoList();
            for (int i = 0; i < count; ++i)
            {
                m_dummyInfoList.Add(NewDummyInfo());
            }
            Refresh(m_dummyInfoList, resetSelect);
        }

        public void Refresh(IList infoList)
        {
            Refresh(infoList, false);
        }

        public void Refresh(IList infoList, bool resetSelect)
        {
            bool forceResetSelect = m_selectType == SelectType.OneJust && (m_infoList.Count == 0);
            m_infoList.Clear();
            if (infoList != null)
            {
                for (int i = 0; i < infoList.Count; ++i)
                {
                    m_infoList.Add(infoList[i]);
                }
            }
            m_infoCount = m_infoList.Count;
            int filledLineCount = Mathf.CeilToInt((float)m_infoCount / constraintCount);
            if (m_disableScrollWhenLessCell && m_scrollRect != null)
            {
                m_scrollRect.normalizedPosition = GetDefaultNormalizedPosition();
                bool isActive = GetViewportLineCount() < filledLineCount;
                m_scrollRect.enabled = isActive;
                if (m_scrollRect.horizontalScrollbar != null)
                {
                    m_scrollRect.horizontalScrollbar.gameObject.SetActive(isActive);
                }
                if (m_scrollRect.verticalScrollbar != null)
                {
                    m_scrollRect.verticalScrollbar.gameObject.SetActive(isActive);
                }
            }
            if (forceResetSelect || resetSelect)
            {
                ResetAllSelect();
            }
            RebuildImmediately();
        }

        public bool IsDrawingInfo(object info)
        {
            for (int i = 0; i < m_cellList.Count; ++i)
            {
                if (m_cellList[i].info == info)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDrawingIndex(int index)
        {
            if (index < m_infoList.Count && index >= 0)
            {
                return IsDrawingInfo(m_infoList[index]);
            }
            return false;
        }

        public UICell GetCellByInfo(object info)
        {
            for (int i = 0; i < m_cellList.Count; ++i)
            {
                if (m_cellList[i].info == info)
                {
                    return m_cellList[i];
                }
            }
            if (m_isStaticInRule)
            {
                for (int i = 0; i < m_headStaticCellList.Count; ++i)
                {
                    if (m_headStaticCellList[i].info == info)
                    {
                        return m_headStaticCellList[i];
                    }
                }
                for (int i = 0; i < m_tailStaticCellList.Count; ++i)
                {
                    if (m_tailStaticCellList[i].info == info)
                    {
                        return m_tailStaticCellList[i];
                    }
                }
            }
            return null;
        }

        public UICell GetCellByIndex(int index)
        {
            if (index < m_infoList.Count && index >= 0)
            {
                return GetCellByInfo(m_infoList[index]);
            }
            return null;
        }

        public void UpdateCell(int index)
        {
            UICell cell = GetCellByIndex(index);
            if (cell != null)
            {
                cell.UpdateCell(m_infoList[index]);
            }
        }

        public void UpdateCell(int index, object info)
        {
            UICell cell = GetCellByIndex(index);
            if (cell != null)
            {
                cell.UpdateCell(info);
            }
        }

        public void ScrollToIndex(int index, ScrollEndAnchor anchor = ScrollEndAnchor.Auto)
        {
            ScrollToIndex(index, 0f, null, anchor);
        }

        public void ScrollToIndex(int index, float time, ScrollEndAnchor anchor = ScrollEndAnchor.Auto)
        {
            ScrollToIndex(index, time, null, anchor);
        }

        public void ScrollToIndex(int index, float time, Action actionOnScrollEnd, ScrollEndAnchor anchor = ScrollEndAnchor.Auto)
        {
            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;
            RectTransform.Axis axis = GetAxis();
            Rect cutRect = GetCutRect();
            Rect contentRect = rectTransform.rect;
            m_startLerpPos = m_scrollRect.normalizedPosition;
            m_targetLerpPos = m_scrollRect.normalizedPosition;
            m_actionOnScrollEnd = actionOnScrollEnd;
            if (anchor == ScrollEndAnchor.Auto)
            {
                anchor = GetAutoAnchor();
            }
            if (axis == RectTransform.Axis.Horizontal)
            {
                int posX = axis == m_startAxis ? index % m_actualCellCountX : index / constraintCount;
                if (cornerX == 1)
                {
                    posX = m_actualCellCountX - 1 - posX;
                }
                float valueX = 0f;
                if (contentRect.width > cutRect.width)
                {
                    if (anchor == ScrollEndAnchor.LowerOrLeft)
                    {
                        valueX = (m_Padding.left + (m_cellSize.x + m_spacing.x) * posX - m_spacing.x) / (contentRect.width - cutRect.width);
                    }
                    else if (anchor == ScrollEndAnchor.UpperOrRight)
                    {
                        valueX = (m_Padding.left + (m_cellSize.x + m_spacing.x) * (posX + 1) - cutRect.width) / (contentRect.width - cutRect.width);
                    }
                    else if (anchor == ScrollEndAnchor.Center)
                    {
                        valueX = (m_Padding.left + (m_cellSize.x + m_spacing.x) * posX - (cutRect.width - m_cellSize.x) / 2f) / (contentRect.width - cutRect.width);
                    }
                    m_targetLerpPos.x = Mathf.Clamp01(valueX);
                }
            }
            else if (axis == RectTransform.Axis.Vertical)
            {
                int posY = axis == m_startAxis ? index % m_actualCellCountY : index / constraintCount;
                if (cornerY == 1)
                {
                    posY = m_actualCellCountY - 1 - posY;
                }
                float valueY = 0f;
                if (contentRect.height > cutRect.height)
                {
                    if (anchor == ScrollEndAnchor.UpperOrRight)
                    {
                        valueY = (m_Padding.top + (m_cellSize.y + m_spacing.y) * posY - m_spacing.y) / (contentRect.height - cutRect.height);
                    }
                    else if (anchor == ScrollEndAnchor.LowerOrLeft)
                    {
                        valueY = (m_Padding.top + (m_cellSize.y + m_spacing.y) * (posY + 1) - cutRect.height) / (contentRect.height - cutRect.height);
                    }
                    else if (anchor == ScrollEndAnchor.Center)
                    {
                        valueY = (m_Padding.top + (m_cellSize.y + m_spacing.y) * posY - (cutRect.height - m_cellSize.y) / 2f) / (contentRect.height - cutRect.height);
                    }
                    m_targetLerpPos.y = Mathf.Clamp01(1 - valueY);
                }
            }
            if (time > 0f && (m_scrollRect.normalizedPosition - m_targetLerpPos).sqrMagnitude > float.Epsilon)
            {
                m_lerpTime = time;
                m_curTime = 0f;
                m_startLerp = true;
            }
            else
            {
                ScrollEnd();
            }
        }

        public void ScrollEnd()
        {
            m_startLerp = false;
            m_scrollRect.normalizedPosition = m_targetLerpPos;
            if (m_actionOnScrollEnd != null)
            {
                m_actionOnScrollEnd();
            }
        }

        public void ScrollToInfo(int index, ScrollEndAnchor anchor = ScrollEndAnchor.Auto)
        {
            ScrollToInfo(index, 0f, null, anchor);
        }

        public void ScrollToInfo(int index, float time, ScrollEndAnchor anchor = ScrollEndAnchor.Auto)
        {
            ScrollToInfo(index, time, null, anchor);
        }

        public void ScrollToInfo(object info, float time, Action actionOnScrollEnd, ScrollEndAnchor anchor)
        {
            int index = m_infoList.IndexOf(info);
            if (index < 0)
            {
                return;
            }
            ScrollToIndex(index, time, actionOnScrollEnd, anchor);
        }

        public void ClearExcludedSelection()
        {
            for (int i = m_selectedInfoList.Count - 1; i >= 0; --i)
            {
                if (!m_infoList.Contains(m_selectedInfoList[i]))
                {
                    m_selectedInfoList.RemoveAt(i);
                }
            }
        }

        public void ResetAllSelect()
        {
            if (m_selectType == SelectType.OneJust)
            {
                if (m_infoList != null && m_infoList.Count > 0)
                {
                    SelectInfo(m_infoList[0]);
                }
            }
            else
            {
                bool isChanged = false;
                for (int i = m_selectedInfoList.Count - 1; i >= 0; --i)
                {
                    isChanged |= SetSelectState(m_selectedInfoList[i], false);
                }
                if (isChanged && eventOnSelectedListChanged != null)
                {
                    eventOnSelectedListChanged();
                }
            }
            RebuildImmediately();
        }

        public bool IsInfoSelected(object info)
        {
            return m_selectedInfoList.Contains(info);
        }

        public bool IsIndexSelected(int index)
        {
            if (index < m_infoList.Count && index >= 0)
            {
                return IsInfoSelected(m_infoList[index]);
            }
            return false;
        }

        public void SelectIndex(int index)
        {
            if (index < m_infoList.Count && index >= 0)
            {
                SelectInfo(m_infoList[index]);
            }
        }

        public void SelectInfo(object info)
        {
            if (m_selectType == SelectType.None)
            {
                return;
            }
            bool isChanged = false;
            if (m_selectType == SelectType.One || m_selectType == SelectType.OneJust)
            {
                if (m_allowSelectSameAgain || !m_selectedInfoList.Contains(info))
                {
                    if (m_selectedInfoList.Count == 0 || SetSelectState(m_selectedInfoList[0], false))
                    {
                        isChanged = SetSelectState(info, true);
                    }
                }
            }
            else if (m_selectType == SelectType.More)
            {
                isChanged = SetSelectState(info, !IsInfoSelected(info));
            }
            else if (m_selectType == SelectType.MoreLimited)
            {
                if (IsInfoSelected(info))
                {
                    isChanged = SetSelectState(info, false);
                }
                else if (m_selectedInfoList.Count < m_selectLimitedCount)
                {
                    isChanged = SetSelectState(info, true);
                }
            }
            if (isChanged && eventOnSelectedListChanged != null)
            {
                eventOnSelectedListChanged();
            }
        }
    }
}