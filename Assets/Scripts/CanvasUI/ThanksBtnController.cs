using RuStore.Example;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт висит на кнопке пожертвовать.
/// При нажатии на кнопку отображает панель с возможными покупками.
/// После чего обрщается к главному скрипту для работы с RustoreSDK и подгружает список возможных товаров.
/// </summary>
class ThanksBtnController : MonoBehaviour
{
    [SerializeField] private ExampleController exampleController;
    [SerializeField] private GameObject PurchaseUI; // UI game object на сцене, который отображает доступные покупки.
    [SerializeField] private Button thanksBtn;

    bool opened = false;



    private void OnEnable()
    {
        thanksBtn.onClick.AddListener(() => {
            if (opened == false) {  // Нажали на кнопку пожертвовать, а панель внутренних покупок была закрыта
                PurchaseUI.SetActive(true);
                exampleController.LoadProducts();
                opened = true;
            }
            else if (opened == true)     // Нажали на кнопку пожертвовать, а панель уже была открыта => ее надо закрыть
            {
                PurchaseUI.SetActive(false);
                opened = false;
            }

        });
    }
    private void OnDisable()
    {
        thanksBtn.onClick.RemoveAllListeners();
    }
}
