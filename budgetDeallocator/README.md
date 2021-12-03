# Overview

![Budget Deallocator Flow](media/budgetDeallocator%20Flow.png)

Budget Deallocator's intent is to protect budgets from crossing over a specified threshold. This is done by taking action to deallocate Azure resources upon being triggered by an Azure Cost Management Budget Alert trigger. 

Generally, a resource which is in the deallocated, paused, shutdown, or similar state in Azure no longer incurs compute charges; however, it is important to understand that not all resources in Azure are eligible to be deallocated, and that other charges for the resources, such as storage, will still incur charges. 

Today, the following resource types are deallocated by Budget Deallocator: 
- Azure Virtual Machines
- Azure Synapse Analytics Dedicated Pools
- Azure Kubernetes Service Clusters

Budget Deallocator has been split into two separate Logic Apps so that an ITSM, workflow manager, or similar may be placed in between to handle additional logic such as an approval workflow.

The first Logic App, **BudgetDeallocatorOrchestrator**, is what is triggered by the Azure Cost Management Budget alert. Its logic is as follows: 

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Orchestrator%20Logic.png)

The second Logic App, **BudgetDeallocator**, is triggered by **BudgetDeallocatorOrchestrator** and does the actual work of identifying active resources that are candidates to be deallocated and changing their state so they no longer incur compute charges. Its logic is as follows:

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Logic.png)

---

## Deploy BudgetDeallocator Logic App

1. From the Azure Portal, click on the **+Create a resource** icon on the top of the far-left navigation blade, search for *Logic App* and select **Logic App** from the results. 
  
    ![Create Logic App](media/CreateLogicApp-01.png)
    
1. Fill out the **Basics** form with the appropriate information following the guidance below: 

    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | Subscritpion  | A centrally managed or "hub" Azure Subscription  |
    | Resource Group  | budgetDeallocator-RG  |
    | Type  | Consumption  |
    | Logic App Name  | budgetDeallocator  |
  
1. Leave all other fields as default, click **Review+Create** and upon validation, click the **Create** button. 

    ![Create Logic App](media/CreateLogicApp-02.png)

 1. From your new Logic App's resource page, click **Identity**, then toggle the *System Assigned Managed Identity* to **On** and click **Save.**
 
     ![Create Logic App](media/CreateLogicApp-07.png)
     
 1. Click **Azure Role Assignments**

 1. Next, click **+ Add role assignment**, select **Subscription** as *Scope*, the appropriate subscription as *Subscription*. Select the custom role created earlier, **Resource Reader and Operator** as *Role.* Then click **Save.**
    >This will allow the Logic App to enumerate resources in your environment.

     ![Create Logic App](media/CreateLogicApp-08.png)
     
---

## Deploy BudgetDeallocatorOrchestrator Logic App

1. From the Azure Portal, click on the **+Create a resource** icon on the top of the far-left navigation blade, search for *Logic App* and select **Logic App** from the results. 
  
    ![Create Logic App](media/CreateLogicApp-01.png)
    
1. Fill out the **Basics** form with the appropriate information following the guidance below: 

    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | Subscritpion  | A centrally managed or "hub" Azure Subscription  |
    | Resource Group  | budgetDeallocator-RG  |
    | Type  | Consumption  |
    | Logic App Name  | budgetDeallocatorOrchestrator  |
  
1. Leave all other fields as default, click **Review+Create** and upon validation, click the **Create** button. 

    ![Create Logic App](media/CreateLogicApp-02.png)
    
 1. From your new Logic App's resource page, click **Identity**, then toggle the *System Assigned Managed Identity* to **On** and click **Save.**
 
     ![Create Logic App](media/CreateLogicApp-03.png)
     
 1. Click **Azure Role Assignments**

 1. Next, click **+ Add role assignment**, select **Subscription** as *Scope*, the appropriate subscription as *Subscription*, and **Reader** as *Role.* Then click **Save.**
    >This will allow the Logic App to enumerate resources in your environment.

     ![Create Logic App](media/CreateLogicApp-04.png)
     
 1. Click **Logic App Code View** from the left-heand navigation blade. Next, select all & delete the default code that is presented on the right-hand side. Once deleted, paste in the JSON contents of the [budgetDeallocatorOrchestrator Logic App found here](https://raw.githubusercontent.com/microsoft/STRIDES/main/budgetDeallocator/budgetDeallocatorOrchestrator.json) and press the **Designer** icon.

     ![Create Logic App](media/CreateLogicApp-05.png)
     
 1. Expand the **Define budgetDeallocatorURI** by clicking on its title and replace the **Value** with the URI for your budgetDeallocator Logic App and click **Save.**
    >Optionally, you can instead replace the value with another API endpoint such as an ITSM solution or other Logic App to extend functionality. 

     ![Create Logic App](media/CreateLogicApp-06.png)
