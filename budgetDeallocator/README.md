# Budget Deallocator
Budget Deallocator's intent is to protect budgets from crossing over a specified threshold. This is done by taking action to deallocate Azure resources upon being triggered by an Azure Cost Management Budget Alert trigger. 

Generally, a resource which is in the deallocated, paused, shutdown, or similar state in Azure no longer incurs compute charges; however, it is important to understand that not all resources in Azure are eligible to be deallocated, and that other charges for the resources, such as storage, will still incur charges. 

Today, the following resource types are deallocated by Budget Deallocator: Azure Virtual Machines, Azure Synapse Analytics Dedicated Pools, and Azure Kubernetes Service Clusters

Budget Deallocator has been split into two separate Logic Apps so that an ITSM, workflow manager, or similar may be placed in between to handle additional logic such as an approval workflow.

The first Logic App, BudgetDeallocatorOrchestrator, is what is triggered by the Azure Cost Management Budget alert. Its logic is as follows: 

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Orchestrator%20Logic.png)

The second Logic App, BudgetDeallocator, is triggered by BudgetDeallocatorOrchestrator and does the actual work of identifying active resources that are candidates to be deallocated and changing their state so they no longer incur compute charges. Its logic is as follows:

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Logic.png)

