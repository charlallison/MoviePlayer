﻿using Common.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualizingListView.Threading.Interface;

namespace VirtualizingListView.Threading
{
    public abstract class Executor : IBackgroundService
    {
        public TaskCollection TaskCollection = new TaskCollection();

        public void Execute()
        {
            var runningTasks = TaskCollection.GetAll().Where(i => !i.IsBusy);
            foreach ( var task in runningTasks)
            {
                task.Completed += Task_Completed;
                task.Started += Task_Started;
                if (!string.IsNullOrEmpty(task.GetMessage())) { }
                    //publish an event...
            }
        }

        private void Task_Started()
        {
            lock (this)
            {
                var runningTasks = TaskCollection.Count();

                if(runningTasks == 0)
                {
                    Console.WriteLine("All processes completed.");
                }
                //else if(runningTasks == 1)
                //{
                //    var task = TaskCollection.GetAll().Where(i => i.IsBusy).First();
                //    EventAggregator.GetEvent<LongProcessStartedEvent>().Publish(task.GetMessage());
                //}
                else if(runningTasks > 1)
                {
                    Console.WriteLine(string.Format("{0} processes running.", runningTasks));
                }
            }
        }

        private void Task_Completed(Task sender)
        {
           // var runningTasks = TaskCollection.GetAll().Where(i => i.IsBusy).Count();
            var runningTasks = TaskCollection.Count();
            if (runningTasks == 0)
            {
                EventAggregator.GetEvent<LongProcessCompletedEvent>().
                    Publish(1);
                Console.WriteLine("All processes completed.");
            }
            else if (runningTasks == 1)
            {
                //EventAggregator.GetEvent<LongProcessStartedEvent>().
                //    Publish(TaskCollection.GetAll().First().GetMessage());
                //publish message
            }
            else if (runningTasks > 1)
            {
                //EventAggregator.GetEvent<LongProcessStartedEvent>().
                //    Publish(string.Format("{0} processing running.", runningTasks));
                Console.WriteLine(string.Format("{0} processes running.", runningTasks));
            }
            TaskCollection.Remove(sender);
        }

        public void Execute(Task task)
        {
            //var found = TaskCollection.GetAll().FirstOrDefault(i => i == task);
            //if (found == null) return;
            task.Run();
        }

        public Task Execute(Action action, string message = null, Action callback = null)
        {
            var task = InitTask(action, message, callback);
            task.Run();
            return task;
        }

        private Task InitTask(Action action, string message, Action callback)
        {
            var task = new Task.TaskImpl(action, message, callback);
            TaskCollection.Add(task);
            return task;
        }

        public Task CreateTask(Action action, string message = null, System.Action callback = null)
        {
            var task = InitTask(action, message, callback);
            task.Started += Task_Started;
            task.Completed += Task_Completed;
            return task;
        }

        public void Shutdown()
        {
            var runningTask = TaskCollection.GetAll().Where(i => i.IsBusy);
            foreach (var item in runningTask)
            {
                item.Stop();
            }
        }

        public void Shutdown(Task task)
        {
            if (task.IsBusy) task.Stop();
        }

        IEventAggregator EventAggregator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventAggregator>();
            }
        }
    }

    public class ExecutorImpl : Executor
    {
        public ExecutorImpl()
        {

        }
    }
}
