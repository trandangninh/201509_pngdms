using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Web.Validators;

namespace Web.Models.QualityAlert
{
    [Validator(typeof(QualityAlertValidator))]
    public class QualityAlertModel : BaseNoisEntityModel
    {
        public DateTime AlertDateTime { get; set; }
        public Line Line { get; set; }
        public string Machine { get; set; }
        public string Detail { get; set; }
        public string Action { get; set; }
        public string GCAS { get; set; }
        public string SAPLot { get; set; }
        public int? NumBlock { get; set; }
        public User Owner { get; set; }
        public string FollowUpAction { get; set; }
        public DateTime When { get; set; }
        public QualityAlertStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public Classification Classification { get; set; }
        public string UserNameCreated { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierLot { get; set; }
        public SupplierCbxModel Supplier { get; set; }
        //public MaterialCbxModel Material { get; set; }
        public string Material { get; set; }
        public string Unit { get; set; }
        public CategoryCbxModel Category { get; set; }
        public ComplaintTypeCbxModel ComplaintType { get; set; }
        public ClassificationDefectCbxModel ClassificationDefect { get; set; }
        public DefectRepeatCbxModel DefectRepeat { get; set; }
        public DateTime SupplierReplyDate { get; set; }
        //public string CostImpacted { get; set; }
        public string PRLossPercent { get; set; }
        public int? QuantityReturn { get; set; }
        public int? NumStop { get; set; }
        public string DownTime { get; set; }
        public int? DefectedQty { get; set; }
        public DateTime InformedToSupplierDate { get; set; }
        public bool RelatedMaterial { get; set; }
        public QualityAlertModel()
        {
            Line = new Line();
            Owner = new User();
            Classification = new Classification();
            Status = new QualityAlertStatus();
            Supplier = new SupplierCbxModel();
            //Material = new MaterialCbxModel();
            Category = new CategoryCbxModel();
            ComplaintType = new ComplaintTypeCbxModel();
            ClassificationDefect = new ClassificationDefectCbxModel();
            DefectRepeat = new DefectRepeatCbxModel();
            FoundByFunction = new FoundByFunction();
        }
        public int? PGerEffortLoss { get; set; }
        public int? ContractorEffortLoss { get; set; }
        public bool? QARelatedToMaterials { get; set; }
        public bool? QARelatedToFG { get; set; }
        public FoundByFunction FoundByFunction { get; set; }
        public int Severity { get; set; }
        public int ClassificationRPN { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User()
        {
            Id = 0;
            Name = "";
        }
        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class Line
    {
        public int Id { get; set; }
        public string LineName { get; set; }
        public Line()
        {
            Id = 0;
            LineName = "";
        }
        public Line(int id, string name)
        {
            Id = id;
            LineName = name;
        }
    }
    public class FoundByFunction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FoundByFunction()
        {
            Id = 0;
            Name = "";
        }
        public FoundByFunction(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class Classification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Classification()
        {
            Id = 0;
            Name = "";
        }
        public Classification(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class QualityAlertStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public QualityAlertStatus()
        {
            Id = 0;
            Name = "";
        }
        public QualityAlertStatus(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class SupplierCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SupplierCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public SupplierCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class MaterialCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MaterialCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public MaterialCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class CategoryCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public CategoryCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ComplaintTypeCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComplaintTypeCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public ComplaintTypeCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class DefectRepeatCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DefectRepeatCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public DefectRepeatCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ClassificationDefectCbxModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ClassificationDefectCbxModel()
        {
            Id = 0;
            Name = "";
        }
        public ClassificationDefectCbxModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class ClassificationRPNModel
    {
        public int Result { get; set; }

        public ClassificationRPNModel()
        {

        }

        public ClassificationRPNModel(int severity, int dectability, int mark)
        {
            Result = (severity * dectability * mark);
        }
    }
}