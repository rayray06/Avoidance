// Nemie default Sprite
//  AA 
// <//> 
//  VV


namespace AvoidanceNEAT.Models.Entity
{
    /// <summary>
    /// Class describing the enemy of the Ero class
    /// </summary>
    public class Nemie: BaseEntity
    {
        /// <summary>
        /// Base constructore of the enemie class
        /// </summary>
        /// <param name="XStartPos">The horizontal starting position of the enemy</param>
        /// <param name="YStartPos">The vertical starting position of the enemy</param>
        /// <param name="XSize">The horizontal size of the enemy</param>
        /// <param name="YSize">The vertical size of the enemy</param>
        /// <param name="XMovement">The horizontal movement of the enemy</param>
        /// <param name="YMovement">The vertical movement of the enemy</param>
        public Nemie(int XStartPos,int YStartPos,int XSize,int YSize,int XMovement = 1,int YMovement = 0):
        base(XStartPos,YStartPos,Nemie.SetSprite(XSize,YSize,XMovement,YMovement),XMovement,YMovement){}

        /// <summary>
        /// Class to define the sprit of the enmie given it's size and movement
        /// </summary>
        /// <param name="XSize">The horizontal size of the enemy</param>
        /// <param name="YSize">The vertcal size of the enemy</param>
        /// <param name="XMov">The Horizontal movement of the enemy</param>
        /// <param name="YMov">The Vertical movement of the enemy</param>
        /// <returns>The sprit for the givene Enemy</returns>
        private static char[,] SetSprite(int XSize,int YSize,int XMov = 1 , int YMov = 0)
        {
            char[,] NewSprite = new char[YSize,XSize];
            
            // We initilize the corner
            NewSprite[0,0] = ' ';
            NewSprite[0,XSize-1] = ' ';
            NewSprite[YSize-1,0] = ' ';
            NewSprite[YSize-1,XSize-1] = ' ';

            // We fill the top and bottom layer
            for(int i = 1; i < XSize-1; i++)
            {
                NewSprite[0,i] = 'A';
                NewSprite[YSize-1,i] = 'V';
            }


            char bodychar = '.';

            // We change the body design depending of the object movement
            if (YMov * XMov < 0)
            {
                bodychar = '/';
            }
            else if (YMov * XMov > 0)
            {
                bodychar = '\\';
            }
            else if (YMov == 0 && XMov != 0)
            {
                bodychar = '-';
            }
            else if (YMov != 0 && XMov == 0)
            {
                bodychar = '|';
            }

            // We fill the each intermediate layer
            for (int i = 1; i < YSize-1; i++)
            {
                NewSprite[i,0] = '<';
                NewSprite[i,XSize-1] = '>';

                for(int j = 1; j < XSize-1; j++)
                {
                    NewSprite[i,j] = bodychar;
                }
            }

            return NewSprite;   
        }

        /// <summary>
        /// Set the sprite for the current enemy
        /// </summary>
        private void SetSprite()
        { 
            Sprite = Nemie.SetSprite(this.Xsize,this.Ysize,this.XMovement,this.YMovement);
        }


        /// <summary>
        /// Redirection process for the enemy
        /// </summary>
        /// <param name="XNewMovement">The new horizontal movement</param>
        /// <param name="YNewMovement">The new vertical movement</param>
        public override void Redirect(int XNewMovement, int YNewMovement)
        {
            base.Redirect(XNewMovement,YNewMovement);
            SetSprite();
        }
    }
}