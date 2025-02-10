using RuStore.BillingClient;
using UnityEngine;
using static System.Net.WebRequestMethods;


class RustoreManager : MonoBehaviour
{
    private void OnEnable()
    {
        RuStoreBillingClient.Instance.CheckPurchasesAvailability(
            onFailure: (error) =>
            {
                // Process error 
            },
            onSuccess: (response) =>
            {
                if (response.isAvailable)
                {
                    Debug.Log("nice");
                }
                else
                {
                    // Process purchases unavailable 
                }
            }
        );

        string[] productsId = { "Thanks" };
        RuStoreBillingClient.Instance.GetProducts(productsId,
            onFailure: (error) => {
                // Process error
            },
            onSuccess: (response) => {
                Debug.Log(response);
            }
        );
    }
}
