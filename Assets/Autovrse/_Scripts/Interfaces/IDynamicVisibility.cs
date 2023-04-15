using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDynamicVisibility
{
    public void Show();
    public void Hide();
    public bool IsHidden { get; }
}
