﻿#region Info
// -----------------------------------------------------------------------
// IPanelDisplay.cs
// 
// Felix Jung 11.06.2023
// -----------------------------------------------------------------------
#endregion
namespace BT.Scripts.Gameplay {
  public interface IPanelDisplay {
    void Initialize();
    void SetActive(bool isActive);
    void UpdatePanel();
  }
}