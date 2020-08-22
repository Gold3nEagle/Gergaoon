using SweetSugar.Scripts.Blocks;
using UnityEngine;

namespace SweetSugar.Scripts.Level
{
    /// <summary>
    /// Outline around the field
    /// </summary>
    public class OutlineBorder
    {
        private int maxRows;
        private int maxCols;
        private FieldBoard _fieldBoard;

        public OutlineBorder(int maxRows, int maxCols, FieldBoard fieldBoard)
        {
            this.maxRows = maxRows;
            this.maxCols = maxCols;
            _fieldBoard = fieldBoard;
            GenerateOutline();
        }

        void GenerateOutline()
    {
        int row = 0;
        int col = 0;
        for (row = 0; row < maxRows; row++)
        { //down
            SetOutline(col, row, 0);
        }
        row = maxRows - 1;
        for (col = 0; col < maxCols; col++)
        { //right
            SetOutline(col, row, 90);
        }
        col = maxCols - 1;
        for (row = maxRows - 1; row >= 0; row--)
        { //up
            SetOutline(col, row, 180);
        }
        row = 0;
        for (col = maxCols - 1; col >= 0; col--)
        { //left
            SetOutline(col, row, 270);
        }
        col = 0;
        for (row = 1; row < maxRows - 1; row++)
        {
            for (col = 1; col < maxCols - 1; col++)
            {
                //  if (GetSquare(col, row).type == SquareTypes.NONE)
                SetOutline(col, row, 0);
            }
        }
    }


    void SetOutline(int col, int row, float zRot)
    {
        Square square = _fieldBoard.GetSquare(col, row, true);
        if (square.type != SquareTypes.NONE)
        {
            if (row == 0 || col == 0 || col == maxCols - 1 || row == maxRows - 1)
            {
                var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);

                outline.transform.localRotation = Quaternion.Euler(0, 0, zRot);
                if (zRot == 0)
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.425f;
                if (zRot == 90)
                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.425f;
                if (zRot == 180)
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.425f;
                if (zRot == 270)
                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.425f;
                if (row == 0 && col == 0)
                {   //top left
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                }
                if (row == 0 && col == maxCols - 1)
                {   //top right
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                }
                if (row == maxRows - 1 && col == 0)
                {   //bottom left
                    outline.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                }
                if (row == maxRows - 1 && col == maxCols - 1)
                {   //bottom right
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                }
            }
            else
            {
                //top left
                if (_fieldBoard.GetSquare(col - 1, row - 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col, row - 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);
                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
                //top right
                if (_fieldBoard.GetSquare(col + 1, row - 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col, row - 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                //bottom left
                if (_fieldBoard.GetSquare(col - 1, row + 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col, row + 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col - 1, row, true).type == SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                }
                //bottom right
                if (_fieldBoard.GetSquare(col + 1, row + 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col, row + 1, true).type == SquareTypes.NONE && _fieldBoard.GetSquare(col + 1, row, true).type == SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline3, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }


            }
        }
        else
        {
            bool corner = false;
            if (_fieldBoard.GetSquare(col - 1, row, true).type != SquareTypes.NONE && _fieldBoard.GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                var outline = Object.Instantiate(_fieldBoard.outline2, square.transform);

                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                corner = true;
            }
            if (_fieldBoard.GetSquare(col + 1, row, true).type != SquareTypes.NONE && _fieldBoard.GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                var outline = Object.Instantiate(_fieldBoard.outline2, square.transform);

                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
                corner = true;
            }
            if (_fieldBoard.GetSquare(col + 1, row, true).type != SquareTypes.NONE && _fieldBoard.GetSquare(col, row - 1, true).type != SquareTypes.NONE)
            {
                var outline = Object.Instantiate(_fieldBoard.outline2, square.transform);

                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
                corner = true;
            }
            if (_fieldBoard.GetSquare(col - 1, row, true).type != SquareTypes.NONE && _fieldBoard.GetSquare(col, row + 1, true).type != SquareTypes.NONE)
            {
                var outline = Object.Instantiate(_fieldBoard.outline2, square.transform);

                outline.transform.localPosition = Vector3.zero;
                outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                corner = true;
            }


            if (!corner)
            {
                if (_fieldBoard.GetSquare(col, row - 1, true).type != SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline1, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.up * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                if (_fieldBoard.GetSquare(col, row + 1, true).type != SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline1, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.down * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
                }
                if (_fieldBoard.GetSquare(col - 1, row, true).type != SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline1, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.left * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                if (_fieldBoard.GetSquare(col + 1, row, true).type != SquareTypes.NONE)
                {
                    var outline = Object.Instantiate(_fieldBoard.outline1, square.transform);

                    outline.transform.localPosition = Vector3.zero + Vector3.right * 0.395f;
                    outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

    }
    }
}