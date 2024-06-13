using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolarionReports.App_GlobalResources;

namespace PolarionReports.Models
{
    public class Tooltip30EA
    {
        public string WorkitemInBin;
        public string AssociatedAdditionalRequirements;
        public string ComponentsWithoutLinkedRequirements;
        public string CoverageReport;
        public string DeferredWorkitems;
        public string ESEAndNotHWSWMech;
        public string IncorrectlyAssociatedAdditionalRequirements;
        public string IncorrectlyAssociatedAdditionalRequirements50HW;
        public string IncorrectlyAssociatedAdditionalRequirements50SW;
        public string InterfaceAndNotHWSWMech;
        public string InterfacesNotReferenced;
        public string InterfacesWithoutLinkedRequirements;
        public string LinkageTo30InterfaceRequirements;
        public string LinkageTo40HardwareRequirements;
        public string LinkageTo40SoftwareRequirements;
        public string LinkageTo50HWArchitecturalDesign;
        public string LinkageTo50SWArchitecturalDesign;
        public string LinkageWithin30ElementArchitecture;
        public string LinkedAdditionalHardwareRequirements;
        public string LinkedAdditionalHardwareRequirements50HW;
        public string LinkedAdditionalInterfaceRequirement;
        public string LinkedAdditionalRequirementsWithInvalidAllocation;
        public string LinkedAdditionalRequirementsWithInvalidAllocation40HW;
        public string LinkedAdditionalRequirementsWithInvalidAllocation40SW;
        public string LinkedAdditionalRequirementsWithInvalidAllocation50HW;
        public string LinkedAdditionalRequirementsWithInvalidAllocation50SW;
        public string LinkedAdditionalSoftwareRequirements;
        public string LinkedAdditionalSoftwareRequirements50SW;
        public string RejectedWorkitems;
        public string RequirementAllocation;
        public string RequirementStatus;
        public string SpecialReports;
        public string StatusConsitencyReport;
        public string UnassociatedAdditionalRequirements;
        public string UnlinkedAdditionalHardwareRequirements;
        public string UnlinkedAdditionalInterfaceRequirements;
        public string UnlinkedAdditionalSoftwareRequirements;
        public string WorkitemsInClarification;
        public string WorkitemsWithESEAndHW;
        public string WorkitemsWithESEAndSW;
        public string WorkItemsWithInvalidAllocation;
        public string WorkItemsWithoutAllocation;
        public string WorkitemsWithPrioritizationShouldHave;
        public string WorkitemsWithStatusDraft;
        public string WorkItemsWithTBD;
        public string InterfacesWithoutConnectedComponents;
        public string HighLevelRequirementsWithoutAnyBreakDownRequirements;
        public string UnreferencedInterfaces;
        public string AllocInvalid;
        public string SafetyRFelevant;
        public string CybersecurityRelevant;

        // 2019-08-09 Verification
        public string C20LinkageToTestCases;
        public string C20UnlinkedElementRequirements;
        public string C20IncorrectlyLinkedElementRequirements;
        public string C20LinkedElementRequirements;
        public string C20ElementRequirementsWithMissingVerificationProperties;
        public string C20Verification;

        public string DocSignatureStatus;

        public Tooltip30EA()
        {
            WorkitemInBin = Tooltips.WorkitemInBin;
            AssociatedAdditionalRequirements = Tooltips.A30AssociatedAdditionalRequirements;
            ComponentsWithoutLinkedRequirements = Tooltips.A30ComponentsWithoutLinkedRequirements;
            CoverageReport = Tooltips.A30CoverageReport;
            DeferredWorkitems = Tooltips.A30DeferredWorkitems;
            ESEAndNotHWSWMech = Tooltips.A30ESEAndNotHWSWMech;
            IncorrectlyAssociatedAdditionalRequirements = Tooltips.A30IncorrectlyAssociatedAdditionalRequirements;
            IncorrectlyAssociatedAdditionalRequirements50HW = Tooltips.A30IncorrectlyAssociatedAdditionalRequirements50HW;
            IncorrectlyAssociatedAdditionalRequirements50SW = Tooltips.A30IncorrectlyAssociatedAdditionalRequirements50SW;
            InterfaceAndNotHWSWMech = Tooltips.A30InterfaceAndNotHWSWMech;
            InterfacesNotReferenced = Tooltips.A30InterfacesNotReferenced;
            InterfacesWithoutLinkedRequirements = Tooltips.A30InterfacesWithoutLinkedRequirements;
            LinkageTo30InterfaceRequirements = Tooltips.A30LinkageTo30InterfaceRequirements;
            LinkageTo40HardwareRequirements = Tooltips.A30LinkageTo40HardwareRequirements;
            LinkageTo40SoftwareRequirements = Tooltips.A30LinkageTo40SoftwareRequirements;
            LinkageTo50HWArchitecturalDesign = Tooltips.A30LinkageTo50HWArchitecturalDesign;
            LinkageTo50SWArchitecturalDesign = Tooltips.A30LinkageTo50SWArchitecturalDesign;
            LinkageWithin30ElementArchitecture = Tooltips.A30LinkageWithin30ElementArchitecture;
            LinkedAdditionalHardwareRequirements = Tooltips.A30LinkedAdditionalHardwareRequirements;
            LinkedAdditionalHardwareRequirements50HW = Tooltips.A30LinkedAdditionalHardwareRequirements50HW;
            LinkedAdditionalInterfaceRequirement = Tooltips.A30LinkedAdditionalInterfaceRequirement;
            LinkedAdditionalRequirementsWithInvalidAllocation = Tooltips.A30LinkedAdditionalRequirementsWithInvalidAllocation;
            LinkedAdditionalRequirementsWithInvalidAllocation40HW = Tooltips.A30LinkedAdditionalRequirementsWithInvalidAllocation40HW;
            LinkedAdditionalRequirementsWithInvalidAllocation40SW = Tooltips.A30LinkedAdditionalRequirementsWithInvalidAllocation40SW;
            LinkedAdditionalRequirementsWithInvalidAllocation50HW = Tooltips.A30LinkedAdditionalRequirementsWithInvalidAllocation50HW;
            LinkedAdditionalRequirementsWithInvalidAllocation50SW = Tooltips.A30LinkedAdditionalRequirementsWithInvalidAllocation50SW;
            LinkedAdditionalSoftwareRequirements = Tooltips.A30LinkedAdditionalSoftwareRequirements;
            LinkedAdditionalSoftwareRequirements50SW = Tooltips.A30LinkedAdditionalSoftwareRequirements50SW;
            RejectedWorkitems = Tooltips.A30RejectedWorkitems;
            RequirementAllocation = Tooltips.A30RequirementAllocation;
            RequirementStatus = Tooltips.A30RequirementStatus;
            SpecialReports = Tooltips.A30SpecialReports;
            StatusConsitencyReport = Tooltips.A30StatusConsitencyReport;
            UnassociatedAdditionalRequirements = Tooltips.A30UnassociatedAdditionalRequirements;
            UnlinkedAdditionalHardwareRequirements = Tooltips.A30UnlinkedAdditionalHardwareRequirements;
            UnlinkedAdditionalInterfaceRequirements = Tooltips.A30UnlinkedAdditionalInterfaceRequirements;
            UnlinkedAdditionalSoftwareRequirements = Tooltips.A30UnlinkedAdditionalSoftwareRequirements;
            WorkitemsInClarification = Tooltips.A30WorkitemsInClarification;
            WorkitemsWithESEAndHW = Tooltips.A30WorkitemsWithESEAndHW;
            WorkitemsWithESEAndSW = Tooltips.A30WorkitemsWithESEAndSW;
            WorkItemsWithInvalidAllocation = Tooltips.A30WorkItemsWithInvalidAllocation;
            WorkItemsWithoutAllocation = Tooltips.A30WorkItemsWithoutAllocation;
            WorkitemsWithPrioritizationShouldHave = Tooltips.A30WorkitemsWithPrioritizationShouldHave;
            WorkitemsWithStatusDraft = Tooltips.A30WorkitemsWithStatusDraft;
            WorkItemsWithTBD = Tooltips.A30WorkItemsWithTBD;
            InterfacesWithoutConnectedComponents = Tooltips.A30InterfacesWithoutConnectedComponents;
            HighLevelRequirementsWithoutAnyBreakDownRequirements = Tooltips.A30HighLevelRequirementsWithoutAnyBreakDownRequirements;
            UnreferencedInterfaces = Tooltips.A30UnreferencedInterfaces;
            AllocInvalid = Tooltips.A30AllocInvalid;
            SafetyRFelevant = Tooltips.C20SafetyRelevant;
            CybersecurityRelevant = Tooltips.C20CybersecurityRelevant;

            C20LinkageToTestCases = Tooltips.C20LinkageToTestCases;
            C20UnlinkedElementRequirements = Tooltips.C20UnlinkedElementRequirements;
            C20IncorrectlyLinkedElementRequirements = Tooltips.C20IncorrectlyLinkedElementRequirements;
            C20LinkedElementRequirements = Tooltips.C20LinkedElementRequirements;
            C20ElementRequirementsWithMissingVerificationProperties = Tooltips.C20ElementRequirementsWithMissingVerificationProperties;
            C20Verification = Tooltips.C20Verification;

            DocSignatureStatus = Tooltips.DocSignatureStatus;
        }
    }
}