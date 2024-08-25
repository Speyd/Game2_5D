using EntityLib;
using System.Data;
using MapLib;
using System.Reflection.Metadata.Ecma335;

namespace ControlLib
{
    public class Control
    {
        private CheckPressed checkPressed = new CheckPressed();
        private ListIgnoreSymbol ignoreSymbol;

        private double moveSpeed;
        private readonly int speedMultiplier;
        private readonly Map map;

        private double entityX;
        private double entityY;
        private int boost = 1;

        public Control(Map map, int screenWidth, double moveSpeed = 0.009)
        {
            this.map = map;
            ignoreSymbol = new ListIgnoreSymbol(map);

            this.moveSpeed = moveSpeed;
            speedMultiplier = screenWidth / 100;
        }

        private bool checkTrueCoordinates()
        {
            foreach (char symbol in ignoreSymbol.ignoreSymbol)
            {
                if (map.MapStr[(int)entityY * map.MapWidth + (int)entityX] == symbol)
                    return true;
            }

            return false;
        }

        private void forwardPress(Entity entity)
        {
            entityX += Math.Sin(entity.EntityA) * speedMultiplier * moveSpeed * boost;
            entityY += Math.Cos(entity.EntityA) * speedMultiplier * moveSpeed * boost;
        }
        private void backPress(Entity entity)
        {
            entityX -= Math.Sin(entity.EntityA) * speedMultiplier * moveSpeed * boost;
            entityY -= Math.Cos(entity.EntityA) * speedMultiplier * moveSpeed * boost;
        }
        private void leftPress(Entity entity)
        {
            entityX += boost * moveSpeed * Math.Sin(entity.EntityA + Math.PI / 2);
            entityY += boost * moveSpeed * Math.Cos(entity.EntityA + Math.PI / 2);
        }
        private void rightPress(Entity entity)
        {
            entityX -= boost * moveSpeed * Math.Sin(entity.EntityA + Math.PI / 2);
            entityY -= boost * moveSpeed * Math.Cos(entity.EntityA + Math.PI / 2);
        }
        private void turnLeftPress(Entity entity)
        {
            entity.EntityA += moveSpeed;
        }
        private void turnRightPress(Entity entity)
        {
            entity.EntityA -= moveSpeed;
        }

        public void makePressed(Entity entity)
        {
            checkPressed.check();
            entityX = entity.EntityX;
            entityY = entity.EntityY;

            boost = 1;

            if (checkPressed.isTurnBoost)
                boost = 2;
            if (checkPressed.isForwardPressed)
                forwardPress(entity);
            if (checkPressed.isBackPressed)
                backPress(entity);
            if (checkPressed.isLeftPressed)
                leftPress(entity);
            if (checkPressed.isRightPressed)
                rightPress(entity);
            if (checkPressed.isTurnLeftPressed)
                turnLeftPress(entity);
            if (checkPressed.isTurnRightPressed)
                turnRightPress(entity);

            if (checkTrueCoordinates() == true)
            {
                entity.EntityX = entityX;
                entity.EntityY = entityY;
            }
        }
    }
}
