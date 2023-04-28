// Shield Sprite
// Vertical ====
// Horizontal ‖
// 


namespace AvoidanceLight.Models.Entity.Items
{
    public class Shield: BaseEntity
    {

        public Shield(int XPosParam,int YPosParam, int Length,bool isVertical , bool Direction):
        base(XPosParam,YPosParam,SetSprite(Length,isVertical,Direction))
        {

        }

        private static char[,] SetSprite(int Length,bool isVertical , bool Direction)
        {
            throw new NotImplementedException();
        }

    }
}