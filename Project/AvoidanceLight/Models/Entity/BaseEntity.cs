
namespace AvoidanceLight.Models.Entity
{
    public abstract class BaseEntity
    {
        public int XPosition { get; protected set;  }
        public int YPosition { get; protected set; }
        public int Xsize { get; protected set; }
        public int Ysize { get; protected set; }
        public char[,] Sprite { get; protected set; } = new char[0,0];

        public int XMovement { get; protected set; }
        public int YMovement { get; protected set; }

        protected BaseEntity(int XPosParam,int YPosParam, char[,] SpriteParam, int XMovParam = 0, int YMovParam = 0)
        {
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

        public virtual void Move()
        {
            this.XPosition += this.XMovement;
            this.YPosition += this.YMovement;
        }

        public void Teleport(int XNewPos,int YNewPos)
        {
            this.XPosition = XNewPos;
            this.YPosition = YNewPos;
        }

        public virtual void Redirect(int XNewMovement, int YNewMovement)
        {
            this.XMovement = XNewMovement;
            this.YMovement = YNewMovement;
        }

    }
}