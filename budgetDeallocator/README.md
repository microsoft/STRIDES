## Tasks

- [Overview](#Overview)
- [Create the Resource Reader and Operator Custom Role](#Create-the-Resource-Reader-and-Operator-Custom-Role)
- [Deploy BudgetDeallocator Logic App](#Deploy-BudgetDeallocator-Logic-App)
- [Deploy BudgetDeallocatorOrchestrator Logic App](#Deploy-BudgetDeallocatorOrchestrator-Logic-App)
- [Create an Azure Cost Management Budget & Alert](#Create-an-Azure-Cost-Management-Budget-and-Alert)

---

# Overview

![Budget Deallocator Flow](media/budgetDeallocator%20Flow.png)

Budget Deallocator's intent is to protect budgets from crossing over a specified threshold. This is done by taking action to deallocate Azure resources upon being triggered by an Azure Cost Management Budget Alert trigger.

Generally, a resource which is in the deallocated, paused, shutdown, or similar state in Azure no longer incurs compute charges. However, it is important to understand that not all resources in Azure are eligible to be deallocated, and that other charges for deallocated resources, such as storage, will continue to accrue.

Today, the following resource types can be deallocated by Budget Deallocator:

- Azure Virtual Machines
- Azure Synapse Analytics Dedicated Pools
- Azure Kubernetes Service Clusters

Budget Deallocator has been split into two separate Logic Apps so that an ITSM, workflow manager, or similar may be placed in between to handle additional logic such as an approval workflow.

The first Logic App, **BudgetDeallocatorOrchestrator**, is triggered by the Azure Cost Management Budget alert. Its logic is as follows: 

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Orchestrator%20Logic.png)

The second Logic App, **BudgetDeallocator**, is triggered by **BudgetDeallocatorOrchestrator** and does the actual work of identifying active resources that are candidates to be deallocated and changing their state so they no longer incur compute charges. Its logic is as follows:

![Budget Deallocator Orchestrator Logic](media/budgetDeallocator%20Logic.png)

---

# Create the Resource Reader and Operator Custom Role

You will need to create a [custom Azure Role Based Access Control Role](https://docs.microsoft.com/azure/role-based-access-control/custom-roles) so that the BudgetDeallocator Logic App's [System Assigned Managed Identity](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/overview) is able to take action on Azure resources on your behalf. This custom role will [clone the Reader role](https://docs.microsoft.com/azure/role-based-access-control/built-in-roles#reader) so that it may enumerate all resources. You will then extend the role to allow it take the following actions: 

- [Microsoft.Compute/virtualMachines/start/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftcompute)
- [Microsoft.Compute/virtualMachines/restart/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftcompute)
- [Microsoft.Compute/virtualMachines/deallocate/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftcompute)
- [Microsoft.Synapse/workspaces/sqlPools/pause/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftsynapse)
- [Microsoft.Synapse/workspaces/sqlPools/resume/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftsynapse)
- [Microsoft.ContainerService/managedClusters/stop/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftcontainerservice)
- [Microsoft.ContainerService/managedClusters/start/action](https://docs.microsoft.com/azure/role-based-access-control/resource-provider-operations#microsoftcontainerservice)

1. From your Azure subscription, click **Access Control (IAM)**, then click **Roles.** Locate the *Reader* role, click on the ellipsis (…) on the far right and select **Clone**.

    ![Clone Reader Role](media/CreateCustomRole-01.png)

1. In the *Basics* tab, enter **Resource Reader and Operator** for the *Custom role name*.

    Feel free to add a description.  
    Leave everything else as default.
  
1. In the *JSON* tab, click **Edit**, add the following at the end of the *"\*/read",* line, and then click **Save**.

    > Note: Please ensure you add the comma after `"\*/read"`.

    ```json
    ,
    "Microsoft.Compute/virtualMachines/start/action",
    "Microsoft.Compute/virtualMachines/restart/action",
    "Microsoft.Compute/virtualMachines/deallocate/action",
    "Microsoft.Synapse/workspaces/sqlPools/pause/action",
    "Microsoft.Synapse/workspaces/sqlPools/resume/action",
    "Microsoft.ContainerService/managedClusters/stop/action",
    "Microsoft.ContainerService/managedClusters/start/action"
    ```

    ![Paste custom actions Reader Role](media/CreateCustomRole-02.png)

1. Click **Review + Create**, followed by **Create**.

---

# Deploy the BudgetDeallocator Logic App

1. From the Azure Portal, click on the **+ Create a resource** icon on the top of the far-left navigation blade, search for *Logic App* and select **Logic App** from the results. 
  
    ![Create Logic App](media/CreateLogicApp-01.png)

1. Fill out the **Basics** form with the appropriate information following the guidance below: 

    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | Subscritpion  | A centrally managed or "hub" Azure Subscription. |
    | Resource Group  | budgetDeallocator-RG<br />(Click **Create new** and follow your organization's [naming convention](https://docs.microsoft.com/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming).) |
    | Type  | Consumption |
    | Logic App Name  | budgetDeallocator<br />(Follow your organization's naming convention.) |
  
1. Leave all other fields as default, click **Review + Create** and upon validation, click the **Create** button.

1. From your new Logic App's resource page, click **Identity**, then toggle the *System Assigned Managed Identity* to **On** and click **Save**. Click **Yes** to confirm.

     ![Create Logic App](media/CreateLogicApp-07.png)

1. Click **Azure Role Assignments**.

1. Next, click **+ Add role assignment**, select **Subscription** as *Scope* and the appropriate subscription as *Subscription*. Select the custom role created earlier, **Resource Reader and Operator** as *Role*. Then click **Save**.

    > This will allow the Logic App to enumerate and deallocate resources in your environment.

    > You might find the custom role at the bottom of the list.

     ![Create Logic App](media/CreateLogicApp-08.png)

1. Click **Logic App Code View** from the left-hand navigation blade. Next, select and delete all default code on the right-hand side. Once deleted, paste in the JSON contents of the [budgetDeallocator Logic App found here](https://raw.githubusercontent.com/microsoft/STRIDES/main/budgetDeallocator/budgetDeallocator.json) and click **Save**.

     ![Create Logic App](media/CreateLogicApp-09.png)

1. Finally, click **Properties** on the left-hand navigation blade and copy the *Access Endpoint* URL to your clipboard.

    Preserve the URL for use in the *BudgetDeallocatorOrchestrator* Logic App.

---

# Deploy the BudgetDeallocatorOrchestrator Logic App

1. From the Azure Portal, click on the **+Create a resource** icon on the top of the far-left navigation blade, search for *Logic App* and select **Logic App** from the results.
  
    ![Create Logic App](media/CreateLogicApp-01.png)

1. Fill out the **Basics** form with the appropriate information following the guidance below:

    | Field Name  | Recommended Value |
    | ------------- | ------------- |
    | Subscription  | Same as above. |
    | Resource Group  | Same as above. |
    | Type  | Consumption. |
    | Logic App Name  | budgetDeallocatorOrchestrator |
  
1. Leave all other fields as default, click **Review + Create** and upon validation, click **Create**.

    ![Create Logic App](media/CreateLogicApp-02.png)

1. From your new Logic App's resource page, click **Identity**, then toggle the *System Assigned Managed Identity* to **On** and click **Save**. Click **Yes** to confirm.

     ![Create Logic App](media/CreateLogicApp-03.png)

1. Click **Azure Role Assignments**.

1. Next, click **+ Add role assignment**, select **Subscription** as *Scope*, the appropriate subscription as *Subscription*, and **Reader** as *Role*. Then click **Save**.
    >This will allow the Logic App to enumerate resources in your environment.

     ![Create Logic App](media/CreateLogicApp-04.png)

1. Click **Logic App Code View** from the left-hand navigation blade. Next, select and delete all default code on the right-hand side. Once deleted, paste in the JSON contents of the [budgetDeallocatorOrchestrator Logic App found here](https://raw.githubusercontent.com/microsoft/STRIDES/main/budgetDeallocator/budgetDeallocatorOrchestrator.json) and click **Designer**.

     ![Create Logic App](media/CreateLogicApp-05.png)

1. Expand the **Define budgetDeallocatorURI** by clicking on its title and replace the **Value** with the URI for your Budget Deallocator Logic App and click **Save**.

    > Optionally, you can instead replace the value with another API endpoint such as an ITSM solution or other Logic App to extend functionality.

     ![Create Logic App](media/CreateLogicApp-06.png)

---

# Create an Azure Cost Management Budget and Alert

In [Azure Cost Management + Billing](https://docs.microsoft.com/azure/cost-management-billing/cost-management-billing-overview), you define [Azure Budgets](https://docs.microsoft.com/azure/cost-management-billing/costs/tutorial-acm-create-budgets) at specific scopes, such as an Enrollment, Management Group, Subscription, and Resource Group. Within these budgets, you can also set thresholds (e.g., 75% of budget) for alerting. Within each threshold, you can send notifications and trigger [Azure Action Groups](https://docs.microsoft.com/azure/azure-monitor/alerts/action-groups). Azure Action Groups can trigger an ITSM solution or REST API; however, you will focus on triggering the budgetDeallocatorOrchestrator Logic App.

1. From the Azure Portal, click **Cost Management + Billing** on the far-left navigation pane. Next, click **Cost Management**, followed by **Budgets**.

1. Set your scope to the appropriate scope you would like to protect against budget overruns. This can be set to an Enrollment, Management Group, Subscription, or Resource Group.

    > The scope you select here must match or be a child of the scope where you assigned the *Reader* and *Resource Reader and Operator* roles to the system assigned managed identities.

     ![Create a Budget](media/CreateBudget-01.png)

1. Click **+ Add**.

1. From the *Create budget* page, give your budget an appropriate *Name*, *Reset period*, and *Amount*. If appropriate, set the *Creation date* to today's date, then click **Next**.

    ![Create a Budget](media/CreateBudget-02.png)

1. Use the following table as *guidance* for the **Alert conditions** section.

    | Field Name  | Recommended Value | Note |
    | ------------- | ------------- | ------------- |
    | Type  | **Actual**  | Triggers when actual consumption (versus forecast) reaches this threshold. |
    | % of budget  | **90**  | Triggers when actual consumption reaches 90% of the budget amount. |

1. Under **Alert recipients (email)**, enter the email address(es) that should receive a notification. Next, click **Manage action group**.

    ![Create a Budget](media/CreateBudget-03.png)

1. From the Action Groups page, click **+ Create**. Fill out the appropriate details in the *Basics* tab and then go to the *Actions* tab.

    > Create the action group in the subscription where the budget is being created.

    ![Create a Budget](media/CreateBudget-04.png)

1. From the *Actions* tab, choose **Logic App** for *Action type*, then select your **budgetDeallocatorOrchestrator** Logic App and click OK.

    > **DO NOT enable the Common Alert Schema.**

1. Next, give your action and appropriate name and click **Review + Create**, followed by **Create**.

    ![Create a Budget](media/CreateBudget-05.png)

1. Close the *Manage action group* window, which will take you back to the *Create budget* page. You will now be able to choose your new action group as an option from within the *Action Group* drop down, then Click **Create**.

    ![Create a Budget](media/CreateBudget-06.png)

# Congratulations!

You now have a Budget Alert trigger that triggers the budgetDeallocatorOrchestrator Logic App that subsequently triggers the budgetDeallocator Logic App to deallocate compute resources in Azure!

For a detailed overview of the solution, please go to the [Overview](#Overview) at the top of the page.
