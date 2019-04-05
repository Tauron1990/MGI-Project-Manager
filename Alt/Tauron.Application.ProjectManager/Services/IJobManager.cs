﻿using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(Namespace = "MGI-Project-Server")]
    public interface IJobManager
    {
        [OperationContract]
        JobItemDto[] GetActiveJobs();

        [OperationContract]
        bool   InsertJob(JobItemDto     jobItem);
        [OperationContract]
        string ValidateJob(JobItemDto   jobItem);
        [OperationContract]
        void  MarkImportent(JobItemDto jobItem);

        [OperationContract]
        bool StateTransition(string name, JobStatus status);

        [OperationContract]
        bool SeCurrentJob(string name);

        [OperationContract]
        JobItemDto GetCurrentJob();
    }
}