using UnityEngine;

//abstract. 설계도로 사용하려고 할때 사용되는 옵션(추상)
public abstract class InteractableObject : MonoBehaviour
{

    // 자식요소들을 각각의 방식으로 구현할 함수.
    public abstract void Interact();
}