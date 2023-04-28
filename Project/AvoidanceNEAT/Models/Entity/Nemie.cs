// Nemie default Sprite
//  AA 
// <//> 
//  VV


namespace AvoidanceLight.Models.Entity
{
    public class Nemie: BaseEntity
    {

        public Nemie(int XStartPos,int YStartPos,int XSize,int YSize,int XMovement = 1,int YMovement = 0):
        base(XStartPos,YStartPos,Nemie.SetSprite(XSize,YSize,XMovement,YMovement),XMovement,YMovement){}

        private static char[,] SetSprite(int XSize,int YSize,int XMov = 1 , int YMov = 0)
        {
            char[,] NewSprite = new char[YSize,XSize];
            
            NewSprite[0,0] = ' ';
            NewSprite[0,XSize-1] = ' ';
            NewSprite[YSize-1,0] = ' ';
            NewSprite[YSize-1,XSize-1] = ' ';

            for(int i = 1; i < XSize-1; i++)
            {
                NewSprite[0,i] = 'A';
                NewSprite[YSize-1,i] = 'V';
            }

            for(int i = 1; i < YSize-1; i++)
            {
                NewSprite[i,0] = '<';
                NewSprite[i,XSize-1] = '>';

                for(int j = 1; j < XSize-1; j++)
                {
                    char bodychar = '-';

                    if(YMov*XMov < 0)
                    {
                        bodychar = '/';
                    }
                    else if(YMov*XMov > 0)
                    {
                        bodychar = '\\';
                    }
                    else if(YMov == 0 && XMov != 0)
                    {
                        bodychar = '-';
                    }
                    else if(YMov != 0 && XMov == 0)
                    {
                        bodychar = '|';
                    }
                    else
                    {
                        bodychar = '.';
                    }

                    NewSprite[i,j] = bodychar;
                }
            }

            return NewSprite;   
        }

        private void SetSprite()
        { 
            Sprite = Nemie.SetSprite(this.Xsize,this.Ysize,this.XMovement,this.YMovement);
        }

        public override void Redirect(int XNewMovement, int YNewMovement)
        {
            base.Redirect(XNewMovement,YNewMovement);
            SetSprite();
        }
    }
}