using AvoidanceNEAT.Models.Entity;
using System.Windows.Input;

namespace AvoidanceNEAT.Models.Tools
{
    public class Playground
    {
        
        public List<BaseEntity> listEntities {get; private set;}
        public Ero currEro {get; private set;}
        public List<Nemie> listNemies {get; private set;}
        public Screen MainScreen {get; private set;}
        private long spawningTime = 100;
        private long currentStep = 0;
        private Random rdn = new Random();

        public Playground(int XScreenSize,int YScreenSize,Ero? firstParent = null,Ero? secondParent = null)
        {
            MainScreen = new Screen(XScreenSize,YScreenSize);
            listNemies = new List<Nemie>();
            listEntities = new List<BaseEntity>();

            this.ReplaceEro(firstParent,secondParent);

            this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
        }

        public void Reset(Ero firstParent,Ero? secondParent = null)
        {
            while(listNemies.Count() > 0)
            {
                RemoveEntity(listNemies[0]);
            }
            currentStep = 0;
            this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,7),rdn.Next(3,7),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
            ReplaceEro(firstParent,secondParent);
        }

        public void NewFrame()
        {
            if(currEro.alive)
            {            
                currentStep++;

                this.MainScreen.InitScreen();

                foreach( Nemie entity in listNemies )
                {
                    entity.Move();

                    if( (entity.XPosition + entity.Xsize > MainScreen.Xsize) || ( entity.XPosition < 0 ) )
                    {
                        entity.Redirect(-entity.XMovement,entity.YMovement);
                    }

                    if( (entity.YPosition + entity.Ysize > MainScreen.Ysize) || ( entity.YPosition < 0 ) )
                    {
                        entity.Redirect(entity.XMovement,-entity.YMovement);
                    }

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

                    MainScreen.AddObject(entity);
                }

                currEro.MakeAMove();

                currEro.Move();
                
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


                if(currEro.alive)
                {
                    foreach(Nemie nemie in listNemies)
                    {
                        if(currEro.Collide(nemie))
                        {
                            currEro.Kill();
                            break;
                        }
                    }

                    if(!currEro.alive)
                    {
                        while(listNemies.Count() > 0)
                        {
                            RemoveEntity(listNemies[0]);
                        }
                    }
                    
                }
                
                MainScreen.AddObject(currEro);


                if(currentStep == spawningTime)
                {
                    currentStep = 0;
                    this.AddNemie(rdn.Next(0,MainScreen.Xsize-9),rdn.Next(0,MainScreen.Ysize-9),rdn.Next(3,5),rdn.Next(3,5),(rdn.Next(0,2)==0)?-1:1,(rdn.Next(0,2)==0)?-1:1);
                }
            }

        }

        private void ReplaceEro(Ero? firstParent = null,Ero? secondParent = null)
        {
            if(currEro != null)
            {
                listEntities.Remove(currEro);
            }

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

            this.currEro = newCommer;
            this.listEntities.Add(newCommer);
        }


        private void AddNemie(int XStartPos,int YStartPos,int XSize,int YSize,int XMovement= 1,int YMovement= 0)
        {
            Nemie newCommer = new Nemie( XStartPos, YStartPos, XSize, YSize, XMovement, YMovement);
            this.listNemies.Add(newCommer);
            this.listEntities.Add(newCommer);
        }

        private void RemoveEntity(BaseEntity DeadOne)
        {
            listEntities.Remove(DeadOne);
            
            if(DeadOne.GetType() == typeof(Ero)){
                this.ReplaceEro();
            }

            if(DeadOne.GetType() == typeof(Nemie)){
                listNemies.Remove((Nemie)DeadOne);
            }
        }

    }
}