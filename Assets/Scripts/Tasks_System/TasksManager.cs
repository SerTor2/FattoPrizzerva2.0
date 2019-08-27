﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tasks
{
    public enum CanvasHosts
    {
        PLAYER,
        ENEMY
    }


    public class TasksManager : MonoBehaviour
    {
        // singleton
        private static TasksManager _instance;
        public static TasksManager Instance { get { return _instance; } }

        [SerializeField] private TasksBlackboard blackboard;

        [Header("References")]
        [SerializeField] private TasksCanvasController playerTasksCanvasController;
        [SerializeField] private TasksCanvasController hostileTasksCanvasController;

        [Header("Perfomance")]
        [SerializeField] private int updatesPerSecond;
        private float currentUpdateTime;
        private float timeForUpdate;
        [SerializeField] private int checksPerSecond;
        private float currentCheckTime;
        private float timeForCheck;

        [Header("Task Lists")]
        [SerializeField] private List<Task> gameTasks;              // list with ALL the game tasks
        [SerializeField] private List<Task> activeTasks;            // tasks being checked
        [SerializeField] private List<Task> achievedTasks;          // tasks completed SUCCESFULLY
        [SerializeField] private List<Task> failedTasks;            // tasks completed in FAILURE



        #region ENGINE METHODS

        private void Awake()
        {
            #region(Singleton Pattern)
            DontDestroyOnLoad(this);
            // Si _instancia tiene una referencia que no somos nosotros nos destruimos 
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
            #endregion

            // null checks
            if (playerTasksCanvasController == null) Debug.LogError("TASKS_MANAGER_NULL: tasksCanvasController");
            if (hostileTasksCanvasController == null) Debug.LogError("TASKS_MANAGER_NULL: hostileTasksCanvasController");


            // Performance setup
            timeForUpdate = ((1.0000f / (float)updatesPerSecond) * 0.6000f);     // ticks per second caching
            timeForCheck = ((1.0000f / (float)checksPerSecond) * 0.6000f);     // ticks per second caching

            TasksBlackboardSetup();

        }



        private void Update()
        {
            currentCheckTime += Time.deltaTime;
            if (currentCheckTime >= timeForCheck)
            {
                foreach (Task task in gameTasks)
                {
                    CheckTask(task);
                }
                currentCheckTime = 0f;
            }

            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime >= timeForUpdate)
            {
                foreach (Task task in gameTasks)
                {
                    TickTask(task);
                }
                currentUpdateTime = 0f;
            }


        }

        #endregion

        #region PUBLIC METHODS

        public List<Task> GetGameTasks()
        {
            return gameTasks;
        }


        public Task GetTask(int _taskID, List<Task> _tasksListToSearchFrom) 
        {
            Task currentTask;

            for (int index = 0; index < gameTasks.Count; index++)
            {
                currentTask = gameTasks[index];

                // if is a complex task we start doing recursion ............................. //
                if (currentTask is ComplexTask)
                {
                    List<Task> childrenTasks = new List<Task>();
                    childrenTasks = (currentTask as ComplexTask).GetTasksList();

                    foreach (Task childTask in childrenTasks)
                    {
                        GetTask(_taskID, childrenTasks);
                    }

                    // If is a simple task we just check if the id coincides targetId == taskId ....... //
                }
                else
                {
                    if (CheckIsDesiredTask(_taskID, currentTask))
                        return currentTask;
                }
            }

            Debug.LogWarning("WARNING: la tarea con indice " + _taskID + " no ha sido encontrada en la lista GAMETASKS");
            return null;
        }


        
        public void ActivateTask(Task _taskToActivate, bool _safeActivation = true)
        {
            // check if task is already active or not (just in case)
            if (_safeActivation)
            {
                if (activeTasks.Contains(_taskToActivate) == false)
                {
                    SetTaskCategory(_taskToActivate, TaskStatus.IN_PROGRESS);
                }
                else
                {
                    Debug.LogError("ERROR-LIST: The task you are trying to add is alredy active");
                }
            }
            else
            {
                // Insecure addition USE WITH CAUTION
                SetTaskCategory(_taskToActivate, TaskStatus.IN_PROGRESS);
            }

        }

        public void SetupTask(Task _task, TaskData _taskSpecificData, CanvasHosts _canvasForHostingTask, bool _addToActiveTasks = true)
        {
            Debug.Log("Settuping task " + _task + " will show on canvas= " + _addToActiveTasks);

            AddToGameTasks(_task);
            _task.Setup(blackboard);            // le damos la informacion COMUN de todas las tareas

            // redundant check because I don't trust you neither me
            if (_task is I_RequiereInitialization)
                (_task as I_RequiereInitialization).SetTaskData(_taskSpecificData);

            // debido a la propia recursividad de este metodo y el hecho que no queremos que todas las subtareas se añadan a las tareas activas utilizaremos el flag
            if (_addToActiveTasks)
            {
                ActivateTask(_task, true);
                AddTaskToCanvas(_task,_canvasForHostingTask);
            }

            // Propagation
            if (_task is ComplexTask)
            {
                // buscamos sus hijos
                foreach (Task task in (_task as ComplexTask).GetTasksList())
                {
                    task.SetIsChildOfTask();
                    SetupTask(task,_canvasForHostingTask, false);
                }
            }
        }
        public void SetupTask(Task _task,CanvasHosts _canvasHostingTask,  bool _addToActiveTasks = true)
        {
            //Debug.Log("Settuping task " + _task + " will show on canvas= " + _addToActiveTasks);

            AddToGameTasks(_task);          
            _task.Setup(blackboard);            // le damos la informacion comun de todas las tareas

            // debido a la propia recursividad de este metodo y el hecho que no queremos que todas las subtareas se añadan a las tareas activas utilizaremos el flag
            if (_addToActiveTasks)
            {
                ActivateTask(_task, true);
                AddTaskToCanvas(_task,_canvasHostingTask);
            }

            // Propagation
            if (_task is ComplexTask)
            {
                // buscamos sus hijos
                foreach (Task task in (_task as ComplexTask).GetTasksList())
                {
                    task.SetIsChildOfTask();
                    SetupTask(task, _canvasHostingTask, false);
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void AddToGameTasks (Task _task, bool _secureAddition = true)
        {
            if (_secureAddition)
            {
                if (!(gameTasks.Contains(_task)))
                    gameTasks.Add(_task);
                else
                {
                    Debug.LogError("TASKS MANAGER: The task you are trying to add to GAME TASKS is already in there");
                }              
            }
            else
            {
                gameTasks.Add(_task);
            }
        }

        private void AddTaskToCanvas(Task _task, CanvasHosts _targetCanvas)
        {
            TasksCanvasController selectedController = null;
            if (_targetCanvas == CanvasHosts.PLAYER)
                selectedController = playerTasksCanvasController;
            else if (_targetCanvas == CanvasHosts.ENEMY)
                selectedController = hostileTasksCanvasController;

            if (selectedController != null)
            {
                selectedController.AddTaskToCanvas(_task);
                selectedController.UpdatetaskStatus(_task);
            }
            else
            {
                Debug.LogError("Controller of type " + _targetCanvas.ToString() + " not set");
            }

        }

        private bool CheckIsDesiredTask(int _targetIndex, Task _taskToCheck)
        {
            // Is it the task we are looking for?
            if (_taskToCheck.GetTaskId() == _targetIndex)
                return true;
            else
                return false;
        }


        private void TasksBlackboardSetup()
        {
            blackboard.Player = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
        }
        
        private bool CheckTask(Task _task)
        {
            _task.Check();

            TaskStatus previousTaskState = _task.GetPreviousTaskState();
            TaskStatus newTaskState = _task.GetCurrentTaskState();
            //Debug.Log("Checking " + _task.name + " with status " + newTaskState);

            if ((int)previousTaskState != (int)newTaskState)
            {
                SetTaskCategory(_task, newTaskState, previousTaskState);
                playerTasksCanvasController.UpdatetaskStatus(_task);

                if (newTaskState == TaskStatus.ACHIEVED)
                    _task.ApplyReward();

                return true;
            }
            else
            {
                // Category change not needed
                return false;
            }
        }

        private void TickTask(Task _task)
        {
            _task.Tick(timeForUpdate);
        }

        private void SetTaskCategory(Task _task, TaskStatus _newStatus, TaskStatus _previousStatus = TaskStatus.NONE)
        {
            // REMOVING OF LIST
            switch (_previousStatus)
            {
                case TaskStatus.NONE:
                    break;
                case TaskStatus.ACHIEVED:
                    activeTasks.Remove(_task);
                    break;
                case TaskStatus.IN_PROGRESS:
                    activeTasks.Remove(_task);
                    break;
                case TaskStatus.FAILED:
                    failedTasks.Remove(_task);
                    break;
                default:
                    Debug.LogError("ERROR_TASKS: Someghing went wrong, make sure the task " + _task.name + " of type " + _task.GetType() + " inherits properly");
                    break;
            }


            // ADDING ON LIST
            switch (_newStatus)
            {
                case TaskStatus.NONE:
                    Debug.LogError("ERROR_TASKS: Someghing went wrong, make sure the task " + _task.name + " of type " + _task.GetType() + " inherits properly");
                    break;

                case TaskStatus.ACHIEVED:
                    achievedTasks.Add(_task);
                    break;

                case TaskStatus.IN_PROGRESS:
                    activeTasks.Add(_task);
                    break;

                case TaskStatus.FAILED:
                    failedTasks.Add(_task);
                    break;

                default:
                    Debug.LogError("ERROR_TASKS: Someghing went wrong, make sure the task " + _task.name + " of type " + _task.GetType() + " inherits properly");
                    break;
            }


        }

        #endregion
    }
}





