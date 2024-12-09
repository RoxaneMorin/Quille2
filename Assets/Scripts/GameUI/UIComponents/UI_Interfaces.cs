using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuilleUI
{
    public interface ISelectable
    {
        void Select();
        void Unselect();
    }

    public interface IForbiddable
    {
        void Forbid(bool isSelected);
        void Permit(bool isSelected);
    }
}
