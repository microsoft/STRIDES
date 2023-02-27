# Overview
All extramural NIH STRIDES environments are required to report their utilization and cost to NIH monthly so that NIH may audit discounts and report to congress. This document provides instructions on automating the reporting to ensure your NIH STRIDES environment remains compliant. 

## Prerequisites:

- [All STRIDES Azure Subscriptions must be isolated within their own Management Group](#STRIDES-Management-Group)
- [An administrative Azure subscription](#STRIDES-Administrative-Subscription)

## Tasks:

1. [Create an Azure Cost Management Export](#Create-a-Cost-Management-Export)
1. [Use Azure Data Share to Push Data to NIH](#Use-Azure-Data-Share-to-Push-Data-to-NIH)


# STRIDES Management Group

A [Management Group]( https://docs.microsoft.com/en-us/azure/governance/management-groups/overview) isolating STRIDES workloads from all other institutional workloads is required so that STRIDES workloads do not co-mingle with unrelated workloads. This will enforce an isolation of cost, policy, permissions, and reduce complexity for reporting and maintenance.

Below is an example of a potential management group hierarchy. Your management group hierarchy does not need to align exactly with the example, its intent is merely to illustrate the separation of all STRIDES-related workloads within the STRIDES enrollment into its own Management Group:

![STRIDES Management Group](media/strides-management-group-hierarchy.png)

Creating a management group is a simple and well documented process:
- [What are Azure management groups?]( https://docs.microsoft.com/en-us/azure/governance/management-groups/overview)
- [Quickstart: Create a Management Group]( https://docs.microsoft.com/en-us/azure/governance/management-groups/create-management-group-portal)

# STRIDES Administrative Subscription

A single administrative subscription within your STRIDES enrollment & management group is required in order to host the resources and logic to report cost and utilizations to NIH.


<s> > **_Note:_**  Like all STRIDES subscriptions, you must first seek NIH approval by filling out the [Subscription Provisionoing Form](../subscription%20provisioning/README.md).</s>
> **_Note:_** NIH Approval is no longer required.


Once you have received approval from NIH, creating the administrative subscription is no different than creating any other Azure subscription. Documentation on creating a new EA subscription [can be found here](https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/create-subscription). 

Requirements for the Administrative Subscription:
- Must reside within your NIH enrollment
- Must reside within your NIH Management Group
 
> **_Note:_** Since all EA subscriptions default to the same name ("Microsoft Azure Enterprise") it is strongly recommended that you immediately change the name to something unique and meaningful (e.g. "STRIDES - Admin")

# Create a Cost Management Export

Azure Cost Management provides the ability to automatically schedule an export of your STRIDES environment's cost and utilization into an Azure Storage Account within your STRIDES Adminsitrative Subscription.

1. From the Azure Portal, click on **Cost Management + Billing**, then **Cost Management**.

    ![STRIDES Cost Management](media/strides-cost-management.png)

1. Ensure your scope is set to your STRIDES Management group. If not, click **Change** next to your current scope, drill down until you see your STRIDES management group and select it. 

    ![STRIDES Change Cost Management Scope](media/strides-change-scope.png)


1. Click on **Exports** from the middle blade and follow the detailed instructions in the following link with these parameters.

    Link: **[Tutorial: Create and manage exported data](https://docs.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-export-acm-data?tabs=azure-portal)**



    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | **Name**  | STRIDESMonthly  |
    | **Export Type**  | Monthly export of last month's cost  |
    | **Start Date**  | Default  |
    | **Storage**  | Create new  |
    | **Subscription**  | Your STRIDES Adminsitrative Subscription  |
    | **Resource group**  | Create new Resource Group called "STRIDES-exports-rg"  |
    | **Account Name**  | Globally unique and meaningful alphanumeric name  |
    | **Location**  | Azure Region closest to your institution  |
    | **Container**  | Your Azure STRIDES Enrollment Number-InstituionName (e.g. <span style="color:red">**00000000-UniversityOfAzure**</span>)|
    | **Directory**  | exports  |

    ![STRIDES Export Parameters](media/strides-export-params.png)



# Use Azure Data Share to Push Data to NIH

Azure Data Share is a fully-managed, zero overhead service that enables organizations to simply and securely share data between Azure environments. 

It uses an invitation system to connect a data provider to data consumer and allows for the movement of data from provider to consumer without the need for developing complex pipelines, sharing secrets, or granting access.

More information on Azure Data Share [can be found here](https://docs.microsoft.com/en-us/azure/data-share/overview).

1.  [Create an Azure Data Share Account](https://docs.microsoft.com/en-us/azure/data-share/share-your-data-portal#create-a-data-share-account) in your **STRIDES-exports-rg** resource group.
 
1.  Create a Share within the Data Share Account with the parameters as defined below.

    [Create Share documention](https://docs.microsoft.com/en-us/azure/data-share/share-your-data-portal#create-a-share)

    **Details**
    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | **Share name**  | Your Azure STRIDES Enrollment Number-InstituionName-share (e.g. <span style="color:red">**00000000-UniversityOfAzure-share**</span>)  |
    | **Share type**  | Snapshot  |
    | **Description**  | STRIDES Monthly Export<br/> Enrollment Number: 00000000<br/> Institution: institutionName |

    **Datasets**

    Choose the storage account and container that was created to store your monthly [cost management exports](#Create-a-Cost-Management-Export).

    **Recipients**

    Leave recipients blank

    **Settings**

    Enable **Snapshot schedule** with **Recurrence** set to *Daily*


1. Invite the NIH to consume your data by executing the following PowerShell command in [Azure Cloud Shell](https://docs.microsoft.com/en-us/azure/cloud-shell/overview):

    ```powershell
    New-AzDataShareInvitation
   -ResourceGroupName STRIDES-exports-rg
   -AccountName <Your Share Account Name>
   -ShareName <Your Azure STRIDES Enrollment Number-InstituionName-share>
   -Name <Your Azure STRIDES Enrollment Number-InstituionName-share>
   -TargetObjectId <Provided by Microsoft NIH STRIDES Team>
   -TargetTenantId <Provided by Microsoft NIH STRIDES Team>
    ```
To receive the TargetObjectId & TargetTenantId above, please [reach out to the Microsoft STRIDES Team](mailto:MSSTRIDES@microsoft.com).
