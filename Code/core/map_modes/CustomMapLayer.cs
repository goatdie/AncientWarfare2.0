namespace Figurebox.core.map_modes;

public class CustomMapLayer : MapLayer
{
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
    public void SetAllDirty()
    {

    }
    public override void update(float pElapsed)
    {
        if (pixels == null)
            createTextureNew();



        base.update(pElapsed);
    }
}