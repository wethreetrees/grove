﻿namespace Grove.Gameplay.Messages
{
  public class AttachmentDetached
  {
    public Card AttachedTo { get; set; }
    public Card Attachment { get; set; }
  }
}