using System;

namespace Docs.Routing
{
    public class DocumentRouteEvent
    {
        public event Action? OnChange;
        public DocumentRouteMetadata? CurrentDocument { get; set; }
        public void TriggerChange(DocumentRouteMetadata? document)
        {
            CurrentDocument = document;
            OnChange?.Invoke();
        }
    }
}
