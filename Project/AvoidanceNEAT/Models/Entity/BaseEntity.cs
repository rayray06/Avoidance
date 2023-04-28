
namespace AvoidanceLight.Models.Entity
{
    /// <summary>
    /// Class defining the base from each entity of the environment
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// The horizontal position of the entity
        /// </summary>
        public int XPosition { get; protected set;  }

        /// <summary>
        /// The vertical position of the entity
        /// </summary>
        public int YPosition { get; protected set; }

        /// <summary>
        /// The horizontal size of the entity
        /// </summary>
        public int Xsize { get; protected set; }

        /// <summary>
        /// The vertical size of the entity
        /// </summary>
        public int Ysize { get; protected set; }

        /// <summary>
        /// Charcter array corresponding to the sprit of the entity
        /// </summary>
        public char[,] Sprite { get; protected set; } = new char[0,0];

        /// <summary>
        /// the horizontal speed of the entity
        /// </summary>
        public int XMovement { get; protected set; }

        /// <summary>
        /// The vertical speed of the entity
        /// </summary>
        public int YMovement { get; protected set; }

        /// <summary>
        /// Base constructor of the entity
        /// </summary>
        /// <param name="XPosParam">The horizontal starting position of the entity</param>
        /// <param name="YPosParam">The vertical starting position of the entity</param>
        /// <param name="SpriteParam">The sprite of the entity</param>
        /// <param name="XMovParam">The horizontal movement of the entity</param>
        /// <param name="YMovParam">The vertical movement of the entity</param>
        protected BaseEntity(int XPosParam,int YPosParam, char[,] SpriteParam, int XMovParam = 0, int YMovParam = 0)
        {
            // Initialization of every element 
            this.XPosition = XPosParam;
            this.Xsize = SpriteParam.GetLength(1);
            this.YPosition = YPosParam;
            this.Ysize = SpriteParam.GetLength(0);
            this.Sprite = new char[this.Ysize,this.Xsize];
            this.XMovement = XMovParam;
            this.YMovement = YMovParam;


            for(int i = 0; i < this.Ysize ;i++)
            {
                for(int j = 0; j < this.Xsize ;j++)
                {
                    this.Sprite[i,j] = SpriteParam[i,j];

                }
            }
        }

        /// <summary>
        /// Checking for the colision between  our entity and another one
        /// </summary>
        /// <param name="otherActor">Other entity to check for</param>
        /// <returns></returns>
        public bool Collide(BaseEntity otherActor)
        {
            return !(this.XPosition+(this.Xsize) <= otherActor.XPosition ||
                    otherActor.XPosition+(otherActor.Xsize) <= this.XPosition  ||
                    this.YPosition+(this.Ysize) <= otherActor.YPosition ||
                    otherActor.YPosition+(otherActor.Ysize) <= this.YPosition) ;
        }

        /// <summary>
        /// Function to define the move the entity with the given speed
        /// </summary>
        public virtual void Move()
        {
            this.XPosition += this.XMovement;
            this.YPosition += this.YMovement;
        }

        /// <summary>
        /// Teleport the entity to a given location
        /// </summary>
        /// <param name="XNewPos">The new horizontal position</param>
        /// <param name="YNewPos">The new vertical position</param>
        public void Teleport(int XNewPos,int YNewPos)
        {
            this.XPosition = XNewPos;
            this.YPosition = YNewPos;
        }

        /// <summary>
        /// Redirect the movement of the entity
        /// </summary>
        /// <param name="XNewMovement"></param>
        /// <param name="YNewMovement"></param>
        public virtual void Redirect(int XNewMovement, int YNewMovement)
        {
            this.XMovement = XNewMovement;
            this.YMovement = YNewMovement;
        }

    }
}