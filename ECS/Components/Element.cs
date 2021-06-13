using System;

namespace Match3
{
    public class Element : CustomComponent
    {
        public Element(int _x, int _y, int _elementType)
        {
            X = _x;
            Y = _y;
            ElementType = _elementType;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Action<int, int> OnSelect;
        public int ElementType { get; }
        public Action<bool> OnMarkRemove;

        public bool ToRemove
        {
            get => toRemove;
            set
            {
                toRemove = value;
                OnMarkRemove?.Invoke(toRemove);
            }
        }

        private bool toRemove;

        public void OnClick()
        {
            OnSelect?.Invoke(X, Y);
        }
    }
}