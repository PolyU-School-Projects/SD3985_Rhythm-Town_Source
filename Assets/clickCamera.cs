using UnityEngine;

public class clickCamera : MonoBehaviour
{
    public GameObject shutter;
    private Animator shutterAnimator;

    private void Start()
    {
        // 获取快门对象的Animator组件
        shutterAnimator = shutter.GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        // 当物体被点击时触发
        shutterAnimator.SetTrigger("click");
        Debug.Log("ada");
    }
}