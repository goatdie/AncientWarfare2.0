using UnityEngine;
namespace Figurebox;

public abstract class AdditionComponent : MonoBehaviour
{
    private bool Initialized;
    private bool IsFirstOpen = true;
    private void Update()
    {
        if (!Initialized) return;
        OnUpdate();
    }
    private void OnEnable()
    {
        if (!Initialized) return;
        if (IsFirstOpen)
        {
            OnFirstOpen();
            IsFirstOpen = false;
        }
        OnNormalOpen();
    }
    private void OnDisable()
    {
        if (!Initialized) return;
        OnClosed();
    }
    public void Initialize()
    {
        Init();
        Initialized = true;
    }
    protected abstract void Init();
    protected virtual void OnNormalOpen()
    {

    }
    protected virtual void OnFirstOpen()
    {

    }
    private void OnUpdate()
    {

    }
    protected virtual void OnClosed()
    {

    }
}