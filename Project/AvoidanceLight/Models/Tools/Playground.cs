using AvoidanceLight.Models.Entity;
using System.Windows.Input;

namespace AvoidanceLight.Models.Tools
{
    /// <summary>
    /// Class describing a playground for a hero
    /// </summary>
    public class Playground
    {
        /// <summary>
        /// the list of entities populating the playground
        /// </summary>
        public List<BaseEntity> listEntities {get; private set;}
        /// <summary>
        /// The hero of the playground
        /// </summary>
        public Ero currEro {get; private set;}
        /// <summary>
        /// The list of enemies of the playground
        /// </summary>
        public List<Nemie> listNemies {get; private set;}
        /// <summary>
        /// The Screen of the playgroung
        /// </summary>
        public Screen MainScreen {get; private set;}

        /// <summary>
        /// The number of frame before spawning the next enemy
        /// </summary>
        private long spawningTime = 100;
        /// <summary>
        /// The number of frame passed
        /// </summary>
        private long currentStep = 0;
        /// <summary>
        /// Random object used throughout the playground
        /// </summary>
        private Random rdn = new Random();

        /// <summary>
        /// Base constructor of the playground
        /// </summary>
        /// <param name="XScreenSize">The horizontal size of the screen</param>
        /// <param name="YScreenSize">The vertical size of the screen</param>
        public Playground(int XScreenSize,int YScreenSize)
        {
            MainScreen = new Screen(XScreenSize,YScreenSize);
            listNemies = new List<Nemie>();
            listEntities = new List<BaseEntity>();

            this.ReplaceEro();

            this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
        }

        /// <summary>
        /// Reset the playground with a new hero from made from the two parent or a copy of the first parent
        /// </summary>
        /// <param name="firstParent">First parent of the new ero</param>
        /// <param name="secondParent">The secon parent of the new ero (null to copy the first parent)</param>
        public void Reset(Ero firstParent,Ero? secondParent = null)
        {
            // We remove every enemy 
            while(listNemies.Count() > 0)
            {
                RemoveEntity(listNemies[0]);
            }

            // we reset the frame number
            currentStep = 0;

            // We a new enemy 
            this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
            // We replace the hero
            ReplaceEro(firstParent,secondParent);
        }

        /// <summary>
        /// We define the next frame of the playground
        /// </summary>
        public void NewFrame()
        {
            // if ero is dead we don't display anything
            if(currEro.alive)
            {            
                currentStep++;

                // We initialise the screen
                this.MainScreen.InitScreen();

                // We move every enemy
                foreach( Nemie entity in listNemies )
                {
                    entity.Move();

                    // if encounter a border bounce 
                    if( (entity.XPosition + entity.Xsize > MainScreen.Xsize) || ( entity.XPosition < 0 ) )
                    {
                        entity.Redirect(-entity.XMovement,entity.YMovement);
                    }

                  
                    if( (entity.YPosition + entity.Ysize > MainScreen.Ysize) || ( entity.YPosition < 0 ) )
                    {
                        entity.Redirect(entity.XMovement,-entity.YMovement);
                    }

                    // if exceed the screen replace at border
                    if(entity.XPosition + entity.Xsize >= MainScreen.Xsize)
                    {
                        entity.Teleport(MainScreen.Xsize-entity.Xsize,entity.YPosition);
                    }
                    else if( entity.XPosition < 0 )
                    {
                        entity.Teleport(0,entity.YPosition);
                    }

                    if(entity.YPosition + entity.Ysize >= MainScreen.Ysize)
                    {
                        entity.Teleport(entity.XPosition,MainScreen.Ysize-entity.Ysize);
                    }
                    else if ( entity.YPosition < 0 )
                    {
                        entity.Teleport(entity.XPosition,0);
                    }

                    // We addd the enemy to the screen
                    MainScreen.AddObject(entity);
                }

                // The ero plan a move 
                currEro.MakeAMove();

                // The ero make the movement
                currEro.Move();
                
                // If the ero exceed the border we put it back at the border
                if(currEro.XPosition + currEro.Xsize > MainScreen.Xsize)
                {
                    currEro.Teleport(MainScreen.Xsize-currEro.Xsize,currEro.YPosition);
                }
                else if( currEro.XPosition < 0 )
                {
                    currEro.Teleport(0,currEro.YPosition);
                }

                if(currEro.YPosition + currEro.Ysize > MainScreen.Ysize)
                {
                    currEro.Teleport(currEro.XPosition,MainScreen.Ysize-currEro.Ysize);
                }
                else if ( currEro.YPosition < 0 )
                {
                    currEro.Teleport(currEro.XPosition,0);
                }

                // We check if the hero collide with any enemy if so kill the hero
                foreach(Nemie nemie in listNemies)
                {
                    if(currEro.Collide(nemie))
                    {
                        currEro.Kill();
                        break;
                    }
                }

                // if the hero died remove every enemy from the playground
                if(!currEro.alive)
                {
                    while(listNemies.Count() > 0)
                    {
                        RemoveEntity(listNemies[0]);
                    }
                }

                // Display the hero on screen
                MainScreen.AddObject(currEro);

                // if the time to spawn an enemy do so
                if(currentStep == spawningTime)
                {
                    currentStep = 0;
                    this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,5),rdn.Next(3,5),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
                }
            }

        }

        /// <summary>
        /// Replace the curent Hero with a new one
        /// </summary>
        /// <param name="firstParent">First parent of the new ero</param>
        /// <param name="secondParent">The secon parent of the new ero (null to copy the first parent)</param>
        private void ReplaceEro(Ero? firstParent = null,Ero? secondParent = null)
        {
            // If Hero exist remove it
            if(currEro != null)
            {
                listEntities.Remove(currEro);
            }

            // Create the new hero
            Ero newCommer;

            if(secondParent != null)
            {
                newCommer = new Ero(0,0,this,firstParent,secondParent);
            }
            else if(firstParent != null)
            {
                newCommer = new Ero(0,0,this,firstParent);
            }
            else
            {
                newCommer = new Ero(0,0,this);
            }

            // add the new hero to the playground
            this.currEro = newCommer;
            this.listEntities.Add(newCommer);
        }

        /// <summary>
        /// Add a new enemy to the playground
        /// </summary>
        /// <param name="XStartPos">The horizontal starting position of the enemy</param>
        /// <param name="YStartPos">The vertical starting position of the enemy</param>
        /// <param name="XSize">The horizontal size of the enemy</param>
        /// <param name="YSize">The vertical size of the enemy</param>
        /// <param name="XMovement">The horizontal movement of the enemy</param>
        /// <param name="YMovement">The vertical movement of the enemy</param>
        private void AddNemie(int XStartPos,int YStartPos,int XSize,int YSize,int XMovement= 1,int YMovement= 0)
        {
            Nemie newCommer = new Nemie( XStartPos, YStartPos, XSize, YSize, XMovement, YMovement);
            this.listNemies.Add(newCommer);
            this.listEntities.Add(newCommer);
        }

        /// <summary>
        /// Remove an entity drom the playground
        /// </summary>
        /// <param name="DeadOne">the entity to remove</param>
        private void RemoveEntity(BaseEntity DeadOne)
        {
            // we remove it from the entity list 
            listEntities.Remove(DeadOne);
            
            // We remove according to the entity type
            if(DeadOne.GetType() == typeof(Ero)){
                this.ReplaceEro();
            }

            if(DeadOne.GetType() == typeof(Nemie)){
                listNemies.Remove((Nemie)DeadOne);
            }
        }

    }
}