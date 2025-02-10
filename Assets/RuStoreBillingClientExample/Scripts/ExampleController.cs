using System;
using UnityEngine;
using RuStore.Example.UI;
using RuStore.BillingClient;

namespace RuStore.Example
{

    /// <summary>
    /// ���� ����� ��������� UI � �����������������, ��������� � ��������� ���������,
    /// ���������� ����, ������������ ������� � ���������� ������ ��� ���������� � RuStore.
    /// </summary>
    public class ExampleController : MonoBehaviour
    {

        // ������ �������.
        public const string ExampleVersion = "6.1.1";

        // ��������������� ���� ��� ID ���������, UI ����������� � ���� ���������.
        [SerializeField]
        private string[] _productIds;

        [SerializeField]
        private CardsView productsView;

        [SerializeField]
        private CardsView purchasesView;

        [SerializeField]
        private MessageBox _messageBox;

        [SerializeField]
        private LoadingIndicator _loadingIndicator;

        /// <summary>
        /// ���������� ��� ������������� �������. �������������� RuStoreBillingClient.
        /// </summary>
        private void Awake()
        {
            RuStoreBillingClient.Instance.Init();
        }

        /// <summary>
        /// ������������ ����������� ������� ��� ������� ���������, ������������� ������� � �.�.
        /// </summary>
        private void Start()
        {
            // ������������ ������� ��� ������� ���������, ������������� ������� � �.�.
            ProductCardView.OnBuyProduct += ProductCardView_OnBuyProduct;
            PurchaseCardView.OnConfirmPurchase += PurchaseCardView_OnConfirmPurchase;
            PurchaseCardView.OnDeletePurchase += PurchaseCardView_OnDeletePurchase;
            PurchaseCardView.OnGetPurchaseInfo += PurchaseCardView_OnGetPurchaseInfo;
        }

        // ���������� ������� ������� ��������.
        private void ProductCardView_OnBuyProduct(object sender, EventArgs e)
        {
            var product = (sender as ICardView<Product>).GetData();
            BuyProduct(product.productId);
        }

        // ���������� ������� ������������� �������.
        private void PurchaseCardView_OnConfirmPurchase(object sender, EventArgs e)
        {
            var purchase = (sender as ICardView<Purchase>).GetData();
            ConsumePurchase(purchase.purchaseId);
        }

        // ���������� ������� �������� �������.
        private void PurchaseCardView_OnDeletePurchase(object sender, EventArgs e)
        {
            var purchase = (sender as ICardView<Purchase>).GetData();
            DeletePurchase(purchase.purchaseId);
        }

        // ���������� ������� ��������� ���������� � �������.
        private void PurchaseCardView_OnGetPurchaseInfo(object sender, EventArgs e)
        {
            var purchase = (sender as ICardView<Purchase>).GetData();
            GetPurchaseInfo(purchase.purchaseId);
        }

        /// <summary>
        /// �������� ������� ������� Escape ��� ������ �� ����������.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// �������� ���� �� ������ ��� ������� � ����������� �� ����������� ��������.
        /// </summary>
        public void CheckTheme(bool value)
        {
            var theme = value ? BillingClientTheme.Dark : BillingClientTheme.Light;
            RuStoreBillingClient.Instance.SetTheme(theme);
        }

        /// <summary>
        /// ��������� ����������� ������� � ������� RuStoreBillingClient.
        /// </summary>
        public void CheckPurchasesAvailability()
        {
            _loadingIndicator.Show();

            RuStoreBillingClient.Instance.CheckPurchasesAvailability(
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: (result) => {
                    _loadingIndicator.Hide();

                    if (result.isAvailable)
                    {
                        _messageBox.Show("Availability", "True");
                    }
                    else
                    {
                        OnError(result.cause);
                    }
                });
        }

        /// <summary>
        /// ��������� �������� �� RuStore � ���������� �� �� UI.
        /// </summary>
        public void LoadProducts()
        {
            _loadingIndicator.Show();

            productsView.gameObject.SetActive(true);
            purchasesView.gameObject.SetActive(false);

            RuStoreBillingClient.Instance.GetProducts(
                productIds: _productIds,
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: (result) => {
                    _loadingIndicator.Hide();
                    productsView.SetData(result);
                });
        }

        /// <summary>
        /// ��������� ������ ������� � ���������� �� �� UI.
        /// </summary>
        public void LoadPurchases()
        {
            _loadingIndicator.Show();

            productsView.gameObject.SetActive(false);
            purchasesView.gameObject.SetActive(true);

            RuStoreBillingClient.Instance.GetPurchases(
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: (result) => {
                    _loadingIndicator.Hide();
                    purchasesView.SetData(result);
                });
        }

        /// <summary>
        /// ������� �������� ����� RuStoreBillingClient.
        /// </summary>
        public void BuyProduct(string productId)
        {
            _loadingIndicator.Show();
            RuStoreBillingClient.Instance.PurchaseProduct(
                productId: productId,
                quantity: 1,
                developerPayload: "test payload",
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: (result) => {
                    _loadingIndicator.Hide();

                    bool isSandbox = false;
                    switch (result)
                    {
                        case PaymentSuccess paymentSuccess:
                            isSandbox = paymentSuccess.sandbox;
                            break;
                        case PaymentCancelled paymentCancelled:
                            isSandbox = paymentCancelled.sandbox;
                            break;
                        case PaymentFailure paymentFailure:
                            isSandbox = paymentFailure.sandbox;
                            break;
                    }

                    if (isSandbox)
                    {
                        ShowToast(string.Format("isSandbox: {0}", isSandbox.ToString()));
                    }
                });
        }

        /// <summary>
        /// ���������� ��������� � ���� ����� �� Android ����������.
        /// </summary>
        public void ShowToast(string message)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject utils = new AndroidJavaObject("com.plugins.billingexample.AndroidUtils"))
            {
                utils.Call("showToast", currentActivity, message);
            }
        }

        /// <summary>
        /// ������������� ����������� �������.
        /// </summary>
        public void ConsumePurchase(string purchaseId)
        {
            _loadingIndicator.Show();
            RuStoreBillingClient.Instance.ConfirmPurchase(
                purchaseId: purchaseId,
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: () => {
                    _loadingIndicator.Hide();
                    LoadPurchases();
                });
        }

        /// <summary>
        /// ������� �������.
        /// </summary>
        public void DeletePurchase(string purchaseId)
        {
            _loadingIndicator.Show();
            RuStoreBillingClient.Instance.DeletePurchase(
                purchaseId: purchaseId,
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: () => {
                    _loadingIndicator.Hide();
                    LoadPurchases();
                });
        }

        /// <summary>
        /// �������� ���������� � �������.
        /// </summary>
        public void GetPurchaseInfo(string purchaseId)
        {
            _loadingIndicator.Show();
            RuStoreBillingClient.Instance.GetPurchaseInfo(
                purchaseId: purchaseId,
                onFailure: (error) => {
                    _loadingIndicator.Hide();
                    OnError(error);
                },
                onSuccess: (response) => {
                    _loadingIndicator.Hide();
                    _messageBox.Show("Purchase", string.Format("Purchase id: {0}", response.purchaseId));
                });
        }

        /// <summary>
        /// ���������� ������, ������������ ��������� �� ������ � ���������� �.
        /// </summary>
        private void OnError(RuStoreError error)
        {
            _messageBox.Show("Error", error.description);

            Debug.LogErrorFormat("{0} : {1}", error.name, error.description);
        }
    }
}
