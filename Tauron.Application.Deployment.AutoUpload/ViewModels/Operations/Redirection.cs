using System;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public sealed class Redirection
    {
        public OperationContextBase RedirectionContext { get; }

        public OperationContextBase? ParentContext { get; set; }

        public Type RedirectionView { get; }

        public RedirectionType RedirectionType { get; }

        public Redirection(OperationContextBase redirectionContext, Type redirectionView, RedirectionType redirectionType)
        {
            RedirectionContext = redirectionContext;
            RedirectionView = redirectionView;
            RedirectionType = redirectionType;
        }
    }
}