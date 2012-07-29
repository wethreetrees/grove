﻿namespace Grove.Core.Controllers.Human
{
  using System.Linq;
  using Ui;
  using Ui.SelectTarget;
  using Ui.Shell;

  public class SacrificePermanents : Controllers.SacrificePermanents
  {
    public ViewModel.IFactory DialogFactory { get; set; }
    public IShell Shell { get; set; }

    protected override void ExecuteQuery()
    {
      var dialog = DialogFactory.Create(
        new UiTargetValidator(
          minTargetCount: Count,
          maxTargetCount: Count,
          text: string.Format("Select {0} permanents to sacrifice.", Count),
          isValid: target => Filter(target) && target.Controller == Controller),
        canCancel: false
        );

      Shell.ShowModalDialog(dialog, DialogType.Small, SelectionMode.SelectTarget);

      Result = dialog.Selection.ToList();
    }
  }
}