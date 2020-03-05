﻿using System;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public sealed class Redirection
    {
        public Redirection(OperationContextBase redirectionContext, Type redirectionView, RedirectionType redirectionType)
        {
            RedirectionContext = redirectionContext;
            RedirectionView = redirectionView;
            RedirectionType = redirectionType;
        }

        public OperationContextBase RedirectionContext { get; }

        public Type RedirectionView { get; }

        public RedirectionType RedirectionType { get; }
    }
}