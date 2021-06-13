using System;
using System.Collections.Generic;
using System.Linq;
using Match3.ECS.Entities;
using Match3.ECS.Systems;
using Match3.Enums;
using Microsoft.Xna.Framework;

namespace Match3
{
    public class LevelScript : CustomComponent
    {
        private Vector2 positionCenter;
        private int cellCnt;
        private int fieldSize;
        private int score;

        private delegate void MethodDelegate();
        private List<MethodDelegate> calls = new List<MethodDelegate>();

        private List<(Vector2 position, int elementType)> bombToAdd = new List<(Vector2 position, int elementType)>();
        private List<(Vector2 position, int elementType, Direction direction)> lineToAdd =
            new List<(Vector2 position, int elementType, Direction direction)>();
        private List<(Destroyer destroyer, Transform Transform)> destroyers = new List<(Destroyer destroyer, Transform Transform)>();

        private Vector2? firstCell;
        private Vector2? secondCell;

        private Entity target;
        private (Entity entity, Element element)[,] _elements;
        private bool isLevelStarted;

        public Action OnLevelStart;
        public Action<int> OnScoreChange;

        public LevelScript(int _cellCnt, int _fieldSize)
        {
            cellCnt = _cellCnt;
            fieldSize = _fieldSize;

            _elements = new (Entity, Element)[cellCnt, cellCnt];
        }

        public override void Start()
        {
            positionCenter = GetEntity().GetComponent<Transform>().Position
                             - new Vector2(cellCnt * fieldSize / 2, cellCnt * fieldSize / 2)
                             + new Vector2(5, 5);

            GenerateField();
            GenerateTarget();

            AnimationSystem.OnAnimationEnd += OnAnimationEnd;

            CheckChains();
        }

        public override void Update(GameTime gameTime)
        {
            //collide destroyers
            for (var i = 0; i < destroyers.Count; i++)
            {
                var (x, y) = GetElementCellByPosition(destroyers[i].Transform.Position);
                var (_, element) = _elements[(int) x, (int) y];

                if (element.ToRemove)
                    continue;

                MarkElementAsRemove(element.X, element.Y);
            }
        }

        private void BlowUp(int x, int y)
        {
            var cellStart = new Vector2(x == 0 ? 0 : x - 1,y == 0 ? 0 : y - 1);
            var cellEnd = new Vector2(x == cellCnt - 1 ? cellCnt - 1 : x + 1,y == cellCnt - 1 ? cellCnt - 1 : y + 1);

            for (var i = (int)cellStart.X; i <= cellEnd.X; i++)
            {
                for (var j = (int)cellStart.Y; j <= cellEnd.Y; j++)
                {
                    var (_, element) = _elements[i, j];

                    if (!element.ToRemove)
                    {
                        MarkElementAsRemove(element.X, element.Y);
                    }
                }
            }
        }

        private void GenerateField()
        {
            for (var i = 0; i < cellCnt; i++)
            {
                for (var j = 0; j < cellCnt; j++)
                {
                    var newElementEntity = ElementFactory.GetRandomElement(i, j,GetElementPositionByCell(i, j));

                    _elements[i, j] = (newElementEntity, newElementEntity.GetComponent<Element>());
                    _elements[i,j].element.OnSelect += OnSelectElement;
                }
            }
        }

        private void GenerateTarget()
        {
            target = EntityFactory.GetImage(
                "target",
                Vector2.One,
                ContentManager.GetTextureRegion(TextureNames.Target));
            target.GetComponent<Transform>().Scale = Positions.targetScale;
            target.isActive = false;
        }

        private void OnAnimationEnd()
        {
            NextCall();
        }

        private void AnimateChains(IEnumerable<(Vector2 firstElement, Vector2 lastElement)> chains)
        {
            foreach (var (firstElement, lastElement) in chains)
            {
                var (x, y) = lastElement - firstElement;

                for (var i = 0; i <= x + y; i++)
                {
                    var element = _elements[
                        (int) firstElement.X + (x > 0 ? i : 0),
                        (int) firstElement.Y + (y > 0 ? i : 0)];

                    MarkElementAsRemove(element.element.X, element.element.Y);
                }
            }
        }

        private void ProcessChains(List<(Vector2 firstElement, Vector2 lastElement)> chains)
        {
            var vChains = new List<(Vector2 firstElement, Vector2 lastElement)>();
            var hChains = new List<(Vector2 firstElement, Vector2 lastElement)>();

            //devide chains for horizontal and vertical again
            //its for optimal find of intersect
            foreach (var c in chains)
            {
                if ((int)c.firstElement.X == (int)c.lastElement.X)
                    vChains.Add(c);
                else
                    hChains.Add(c);
            }

            //find special bombs from intersect chains
            var processedChains = new List<(Vector2 firstElement, Vector2 lastElement)>();

            foreach (var vChain in vChains)
            {
                foreach (var hChain in hChains)
                {
                    var intersection = IntersectChains(vChain, hChain);

                    if (intersection != null)
                    {
                        bombToAdd.Add((
                            (Vector2) intersection,
                            _elements[(int) vChain.firstElement.X, (int) vChain.firstElement.Y].element.ElementType));

                        //save chains which shouldn't generate bonuses
                        if (!processedChains.Contains(vChain)) processedChains.Add(vChain);
                        if (!processedChains.Contains(hChain)) processedChains.Add(hChain);
                    }
                }
            }

            //generate bonuses from remaining chains
            var actualChains = chains.Except(processedChains);

            foreach (var chain in actualChains)
            {
                var lineLength = ChainLength(chain);

                if (lineLength >= 4)
                {
                    var linePosition = Vector2.Zero;

                    //check if new line on chain with target elements
                    if (firstCell != null && IsPositionFromChain(chain, (int)firstCell.Value.X, (int)firstCell.Value.Y))
                    {
                        linePosition = (Vector2) firstCell;
                    } else if (secondCell != null && IsPositionFromChain(chain, (int) secondCell.Value.X, (int) secondCell.Value.Y))
                    {
                        linePosition = (Vector2) secondCell;
                    }
                    else
                    {
                        linePosition = GetRandomPositionFromChain(chain);
                    }

                    //if might of chain == 4 then line else bomb
                    if (lineLength == 4)
                        lineToAdd.Add((
                            linePosition,
                            _elements[(int) chain.firstElement.X, (int) chain.firstElement.Y].element.ElementType,
                            ChainDirection(chain)));
                    else
                        bombToAdd.Add((
                            GetRandomPositionFromChain(chain),
                            _elements[(int) chain.firstElement.X, (int) chain.firstElement.Y].element.ElementType));
                }
            }

            //check elements from lines to remove and activate bonuses
            foreach (var (firstElement, lastElement) in chains)
            {
                var (x, y) = lastElement - firstElement;

                for (var i = 0; i <= x + y; i++)
                {
                    var cx = (int) firstElement.X + (x > 0 ? i : 0);
                    var cy = (int) firstElement.Y + (y > 0 ? i : 0);

                    _elements[cx, cy].element.ToRemove = true;
                }
            }

            //clear target elements for next checks
            firstCell = null;
            secondCell = null;
        }

        private void RecalculateField()
        {
            for (var i = 0; i < cellCnt; i++)
            {
                for (var j = 0; j < cellCnt; j++)
                {
                    if (_elements[i, j].entity != null && _elements[i, j].element.ToRemove)
                    {
                        _elements[i, j].entity.Destroy();
                        _elements[i, j].element.OnSelect -= OnSelectElement;
                        _elements[i, j] = (null, null);
                    }
                }
            }

            NextCall();
        }


        private void MoveElement(Entity entity, int x, int y)
        {
            var element = entity.GetComponent<Element>();

            element.X = x;
            element.Y = y;

            entity.GetComponent<Animator>().newPosition = GetElementPositionByCell(x, y);

            _elements[x, y] = (entity, element);
        }

        private void GenerateBonuses()
        {
            //generate bombs
            foreach (var (coord, typeElement) in bombToAdd)
            {
                if (_elements[(int) coord.X, (int) coord.Y].entity != null)
                    continue;

                var newElement = ElementFactory.GetBombElement((int) coord.X, (int) coord.Y, typeElement);

                newElement.GetComponent<Transform>().Position = GetElementPositionByCell((int) coord.X, (int) coord.Y);
                newElement.GetComponent<Animator>().newPosition = GetElementPositionByCell((int) coord.X, (int) coord.Y);
                newElement.GetComponent<Element>().OnSelect += OnSelectElement;
                newElement.GetComponent<BombBonus>().OnActivate += OnActivateBomb;

                _elements[(int) coord.X, (int) coord.Y] = (newElement, newElement.GetComponent<Element>());
            }

            bombToAdd.Clear();

            //generate lines
            foreach (var ((x, y), typeElement, direction) in lineToAdd)
            {
                if (_elements[(int) x, (int) y].entity != null)
                    continue;

                var newElement = ElementFactory.GetLineElement((int) x, (int) y, typeElement, direction);

                newElement.GetComponent<Transform>().Position = GetElementPositionByCell((int) x, (int) y);
                newElement.GetComponent<Animator>().newPosition = GetElementPositionByCell((int) x, (int) y);
                newElement.GetComponent<Element>().OnSelect += OnSelectElement;
                newElement.GetComponent<LineBonus>().OnActivate += OnActivateLine;

                _elements[(int) x, (int) y] = (newElement, newElement.GetComponent<Element>());
            }

            lineToAdd.Clear();

            NextCall();
        }

        private void OnActivateBomb(Element element)
        {
            var explosion = ElementFactory.GetExplosion(element.X, element.Y);
            explosion.GetComponent<Explosion>().OnExplode += BlowUp;
        }

        private void OnActivateLine(Element element, Direction direction)
        {
            if (direction == Direction.vertical)
            {
                if (element.Y != 0)
                    GetDestroyer(element, direction).GetComponent<Animator>().newPosition = GetElementPositionByCell(element.X, 0);

                if (element.Y != cellCnt - 1)
                    GetDestroyer(element, direction).GetComponent<Animator>().newPosition = GetElementPositionByCell(element.X, cellCnt - 1);
            }
            else
            {
                if (element.X != 0)
                    GetDestroyer(element, direction).GetComponent<Animator>().newPosition = GetElementPositionByCell(0, element.Y);
                if (element.X != cellCnt - 1)
                    GetDestroyer(element, direction).GetComponent<Animator>().newPosition = GetElementPositionByCell(cellCnt - 1, element.Y);
            }
        }

        private Entity GetDestroyer(Element element, Direction direction)
        {
            var destroyer = ElementFactory.GetDestroyer(GetElementPositionByCell(element.X, element.Y));
            destroyer.GetComponent<Destroyer>().OnDestroy += OnDestroyDestroyer;
            destroyers.Add((destroyer.GetComponent<Destroyer>(), destroyer.GetComponent<Transform>()));

            return destroyer;
        }

        private void OnDestroyDestroyer(int _entityId)
        {
            destroyers.First(d => d.destroyer.entityId == _entityId).destroyer.OnDestroy -= OnDestroyDestroyer;
            destroyers.RemoveAll(d => d.destroyer.entityId == _entityId);
        }

        private void FallElements()
        {
            for (var i = 0; i < cellCnt; i++)
            {
                var vElement = new List<(Entity entity, Element element)>();

                //define remaining elements
                for (var j = 0; j < cellCnt; j++)
                {
                    if (_elements[i, j].entity != null)
                        vElement.Add(_elements[i, j]);
                }

                //move elements
                var newElementVIndex = 0;

                for (var j = cellCnt - 1; j >= 0 ; j--)
                {
                    if (vElement.Any())
                    {
                        var entity = vElement.Last().entity;
                        MoveElement(entity, i, j);

                        vElement.RemoveAt(vElement.Count - 1);
                    }
                    else
                    {
                        //add missing elements
                        var newElementEntity = ElementFactory.GetRandomElement(i, j, GetElementPositionByCell(i, --newElementVIndex));

                        newElementEntity.GetComponent<Animator>().newPosition = GetElementPositionByCell(i, j);
                        newElementEntity.GetComponent<Element>().OnSelect += OnSelectElement;

                        _elements[i, j] = (newElementEntity, newElementEntity.GetComponent<Element>());
                    }
                }
            }
        }

        private void CheckChains()
        {
            LockUI();

            var vChains = new List<(Vector2 firstElement, Vector2 lastElement)>();
            var hChains = new List<(Vector2 firstElement, Vector2 lastElement)>();

            //find all chains
            for (var i = 0; i < cellCnt; i++)
            {
                var vLineCnt = 0;
                var vLineType = _elements[i, 0].element.ElementType;
                var hLineCnt = 0;
                var hLineType = _elements[0, i].element.ElementType;

                for (var j = 1; j < cellCnt; j++)
                {
                    if (_elements[i, j].element.ElementType == vLineType)
                        vLineCnt++;
                    else
                    {
                        if (vLineCnt > 1)
                            vChains.Add((new Vector2(i, j - vLineCnt - 1), new Vector2(i, j - 1)));

                        vLineCnt = 0;
                        vLineType = _elements[i, j].element.ElementType;
                    }

                    if (_elements[j, i].element.ElementType == hLineType)
                        hLineCnt++;
                    else
                    {
                        if (hLineCnt > 1)
                            hChains.Add((new Vector2(j - hLineCnt - 1, i ), new Vector2(j - 1, i )));

                        hLineCnt = 0;
                        hLineType = _elements[j, i].element.ElementType;
                    }

                    if (j + 1 != cellCnt)
                        continue;

                    if (vLineCnt > 1) vChains.Add((new Vector2(i, j - vLineCnt), new Vector2(i, j)));
                    if (hLineCnt > 1) hChains.Add((new Vector2(j - hLineCnt, i), new Vector2(j, i)));
                }
            }

            vChains.AddRange(hChains);

            if (vChains.Any())
            {
                ProcessChains(vChains);
                AnimateChains(vChains);

                calls.AddRange(new List<MethodDelegate> {
                    RecalculateField,
                    GenerateBonuses,
                    FallElements,
                    RecheckChains});
            }
            else
            {
                UnlockUI();
            }
        }

        private void RecheckChains()
        {
            CheckChains();
        }

        private Vector2 GetElementPositionByCell(int x, int y)
        {
            return positionCenter + new Vector2(fieldSize * x, fieldSize * y);
        }

        private Vector2 GetElementCellByPosition(Vector2 position)
        {
            var (x, y) = (position - positionCenter) / new Vector2(fieldSize, fieldSize);
            return new Vector2((int) x, (int) y);
        }

        private void OnSelectElement(int x, int y)
        {
            //choose first element
            if (!target.isActive || firstCell == new Vector2(x, y))
            {
                target.isActive = true;
                target.GetComponent<Transform>().Position = GetElementPositionByCell(x, y) - Positions.targetOffsetScale;

                firstCell = new Vector2(x, y);
            }
            //switch elements
            else if (IsNearTargetElements(x, y))
            {
                secondCell = new Vector2(x, y);
                target.isActive = false;

                SwitchTargetElements();
            }
            //cancel selection
            else
            {
                firstCell = null;
                target.isActive = false;
            }
        }

        private void SwitchTargetElements()
        {
            var firstElement = _elements[(int)firstCell.Value.X, (int)firstCell.Value.Y].entity;
            var secondElement = _elements[(int)secondCell.Value.X, (int)secondCell.Value.Y].entity;

            MoveElement(firstElement, (int)secondCell.Value.X, (int)secondCell.Value.Y);
            MoveElement(secondElement, (int)firstCell.Value.X, (int)firstCell.Value.Y);

            if (IsElementSpawnsChain((int) firstCell.Value.X, (int) firstCell.Value.Y) ||
                IsElementSpawnsChain((int) secondCell.Value.X, (int) secondCell.Value.Y))
            {
                CheckChains();
            }
            else
            {
                calls.Add(RevertSwitchTargetElements);
            }
        }

        private void RevertSwitchTargetElements()
        {
            var firstElement = _elements[(int)firstCell.Value.X, (int)firstCell.Value.Y].entity;
            var secondElement = _elements[(int)secondCell.Value.X, (int)secondCell.Value.Y].entity;

            MoveElement(firstElement, (int)secondCell.Value.X, (int)secondCell.Value.Y);
            MoveElement(secondElement, (int)firstCell.Value.X, (int)firstCell.Value.Y);
        }

        private bool IsElementSpawnsChain(int xElement, int yElement)
        {
            var elementType = _elements[xElement, yElement].element.ElementType;
            var hCount = 0;
            var vCount = 0;

            if (xElement < cellCnt)
                for (var i = xElement + 1; i < cellCnt; i++)
                    if (_elements[i, yElement].element.ElementType == elementType)
                        hCount++;
                    else
                        break;

            if (xElement > 0)
                for (var i = xElement - 1; i >= 0; i--)
                    if (_elements[i, yElement].element.ElementType == elementType)
                        hCount++;
                    else
                        break;

            if (hCount > 1)
                return true;

            if (yElement < cellCnt)
                for (var i = yElement + 1; i < cellCnt; i++)
                    if (_elements[xElement, i].element.ElementType == elementType)
                        vCount++;
                    else
                        break;

            if (yElement > 0)
                for (var i = yElement - 1; i >= 0; i--)
                    if (_elements[xElement, i].element.ElementType == elementType)
                        vCount++;
                    else
                        break;

            return vCount > 1;
        }

        private void NextCall()
        {
            if (!calls.Any())
            {
                return;
            }

            var call = calls.First();
            calls.Remove(calls.First());
            call.Invoke();
        }

        private bool IsNearTargetElements(int x, int y)
        {
            var dX = Math.Abs(firstCell.Value.X - x);
            var dY = Math.Abs(firstCell.Value.Y - y);

            return dX == 1 && dY == 0 || dX == 0 && dY == 1;
        }

        public override void Destroy()
        {
            calls.Clear();
            target.Destroy();

            for (var i = 0; i < cellCnt; i++)
            {
                for (var j = 0; j < cellCnt; j++)
                {
                    _elements[i, j].entity.Destroy();
                    _elements[i, j].element.OnSelect -= OnSelectElement;
                    _elements[i, j] = (null, null);
                }
            }

            UnlockUI();
        }

        // ReSharper disable once UseDeconstructionOnParameter
        private static int ChainLength((Vector2 f, Vector2 e) l)
        {
            return ((int)l.f.X == (int)l.e.X ? (int)(l.e.Y - l.f.Y) : (int)(l.e.X - l.f.X)) + 1;
        }

        private static Direction ChainDirection((Vector2 f, Vector2 e) l)
        {
            return (int)l.f.X == (int)l.e.X ? Direction.horizontal : Direction.vertical;
        }

        private static Vector2 GetRandomPositionFromChain((Vector2 f, Vector2 l) c)
        {
            return (int) c.f.X == (int) c.l.X
                ? new Vector2(c.f.X, new Random().Next((int) c.f.Y, (int) c.l.Y))
                : new Vector2(new Random().Next((int) c.f.X, (int) c.l.X), c.f.Y);
        }

        private static Vector2? IntersectChains(
            (Vector2 firstElement, Vector2 lastElement) vLine,
            (Vector2 firstElement, Vector2 lastElement) hLine)
        {
            var (vf, vl) = vLine;
            var (hf, hl) = hLine;

            var p = new Vector2(vf.X, hl.Y);

            if (p.X >= hf.X && p.X <= hl.X && p.Y >= vf.Y && p.Y <= vl.Y) return p;

            return null;
        }

        private static bool IsPositionFromChain((Vector2 f, Vector2 e) p, int x, int y)
        {
            return x >= p.f.X && x <= p.e.X && y >= p.e.Y && y <= p.e.Y;
        }

        private void LockUI()
        {
            InputSystem.IsActive = false;
        }

        private void UnlockUI()
        {
            if (!isLevelStarted)
            {
                isLevelStarted = true;
                OnLevelStart?.Invoke();
            }

            InputSystem.IsActive = true;
        }

        private void MarkElementAsRemove(int x, int y)
        {
            _elements[x, y].entity.GetComponent<Animator>().newScale = Vector2.Zero;
            _elements[x, y].element.ToRemove = true;

            if (!isLevelStarted)
                return;

            score += 50;
            OnScoreChange?.Invoke(score);
        }
    }
}