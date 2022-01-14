# Overview
All extramural NIH STRIDES environments are required to report their utilization and cost to NIH monthly so that NIH may audit discounts and report to congress. This document provides instructions on automating the reporting to ensure your NIH STRIDES environment remains compliant. 

Prerequisites:

- [All STRIDES Azure Subscriptions must be isolated within their own Management Group](STRIDES-Management-Group)
- [An administrative Azure subscription within the NIH Management Group](STRIDES-Administrative-Subscription)
- Required resources to request from NIH:
  - Reporting Storage Account name
  - Reporting Storage Account container name
  - Reporting Storage Account SAS token


# STRIDES Management Group

A [Management Group]( https://docs.microsoft.com/en-us/azure/governance/management-groups/overview) isolating STRIDES workloads from all other institutional workloads is required so that STRIDES workloads do not co-mingle with unrelated workloads. This will enforce an isolation of cost, policy, permissions, and reduce complexity for reporting and maintenance.

Below is an example of a potential management group hierarchy. Your management group hierarchy does not need to align exactly with the example, its intent is merely to illustrate the separation of all STRIDES-related workloads within the STRIDES enrollment into its own Management Group:

![STRIDES Management Group](media/strides-management-group-hierarchy.png)

Creating a management group is a simple and well documented process:
- [What are Azure management groups?]( https://docs.microsoft.com/en-us/azure/governance/management-groups/overview)
- [Quickstart: Create a Management Group]( https://docs.microsoft.com/en-us/azure/governance/management-groups/create-management-group-portal)

# STRIDES Administrative Subscription

A single administrative subscription within your STRIDES enrollment & management group is required in order to host the resources and logic to report cost and utilizations to NIH.


> **_Note:_**  Like all STRIDES subscriptions, you must first seek NIH approbval by filling out the Subscription Request form. Please reach out to your Microsoft account team for more information.


Once you have received approval from NIH, creating the administrative subscription is no different than creating any other Azure subscription. Documentation on creating a new EA subscription [can be found here](https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/create-subscription). 

Requirements for the Administrative Subscription:
- Must reside within your NIH enrollment
- Must reside within your NIH Management Group
 
> **_Note:_** Since all EA subscriptions default to the same name ("Microsoft Azure Enterprise") it is strongly recommended that you immediately change the name to something unique and meaningful (e.g. "STRIDES - Admin")
