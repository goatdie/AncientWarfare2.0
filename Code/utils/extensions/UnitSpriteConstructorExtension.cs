using UnityEngine;

namespace Figurebox.Utils.extensions;

public static class UnitSpriteConstructorExtension
{
    public static Sprite GetSpriteUnit(AnimationFrameData pFrameData, Sprite pBody, Sprite pHead,
        ActorAsset pActorAsset,
        ColorAsset pColorAsset, int pSkinSet, int pSkinColor, UnitTextureAtlasID pTextureAtlasID)
    {
        if (pSkinSet < 0) pSkinSet = 0;

        var color_num = pColorAsset != null ? pColorAsset.index_id + 1 : 0L;
        var skin_color_num = pSkinColor != -1 ? pSkinColor + 1 : 0L;
        var skin_set_num = pSkinColor != -1 ? pSkinSet + 1 : 0L;
        long body_num = UnitSpriteConstructor.getBodySpriteSmallID(pBody);

        var head_num = 0;
        if (pHead != null)
        {
            ActorAnimationLoader.int_ids_heads.TryGetValue(pHead, out head_num);
            if (head_num == 0)
            {
                head_num = ActorAnimationLoader.int_ids_heads.Count + 1;
                ActorAnimationLoader.int_ids_heads.Add(pHead, head_num);
            }
        }

        var hash = color_num * 100000000L + head_num * 100000L + body_num * 100L + skin_set_num * 10L + skin_color_num;
        if (!UnitSpriteConstructor._sprites_units.TryGetValue(hash, out var ret_sprite))
        {
            ret_sprite = UnitSpriteConstructor.createNewSpriteUnit(pFrameData, pBody, pHead, pColorAsset, pActorAsset,
                pSkinSet, pSkinColor, pTextureAtlasID);
            UnitSpriteConstructor._sprites_units[hash] = ret_sprite;
        }

        return ret_sprite;
    }
}