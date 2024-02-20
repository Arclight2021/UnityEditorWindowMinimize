using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorWindowMinimize
{
    public class EditorWindowMinimize : EditorWindow
    {
        [Serializable]
        public class WindowInfo
        {
            public EditorWindow window;
            public string name;
            public Rect position;
            public bool isMinimize;
            public bool isClosed => window == null;
            public Vector2 minSize;
            public string windowType;
            public string assemblyFullName;

            public Button btn;
        }

        private Label windowNameLabel;
        private VisualElement windowBtnContainer;
        public bool _isSelecting;
        private Button selectBtn;

        public List<WindowInfo> windowInfos = new();
        public Rect minimizePos = new Rect(0, 0, 10, 10);

        private readonly Color closeColor = Color.red;
        private readonly Color minimizeColor = Color.cyan;
        private readonly Color maximizeColor = Color.green;

        [MenuItem("EditorWindowMinimize/Open")]
        public static void OpenEasyWindow()
        {
            var window = GetWindow<EditorWindowMinimize>();
            window.titleContent = new GUIContent("EditorWindowMinimize");
        }

        private void OnEnable()
        {
            var topContainer = CreateTopContainer();

            CreateWindowBtnContainer();

            rootVisualElement.Add(topContainer);
            rootVisualElement.Add(windowBtnContainer);

            if (windowInfos.Count >= 1)
            {
                UpdateView();
            }
        }
        
        private VisualElement CreateTopContainer()
        {
            var topContainer = new VisualElement
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row
                }
            };

            windowNameLabel = new Label
            {
                name = "WinLabel",
                style =
                {
                    width = 200
                }
            };

            selectBtn = new Button
            {
                text = "SelectWindow"
            };
            selectBtn.clicked += () => _isSelecting = true;

            var refreshBtn = new Button
            {
                text = "Refresh",
                tooltip = "Recreate all window btn"
            };
            refreshBtn.clicked += UpdateView;

            var clearBtn = new Button
            {
                text = "Clear"
            };
            clearBtn.clicked += () =>
            {
                windowInfos.Clear();
                UpdateView();
            };

            var setPosBtn = new Button
            {
                text = "Set Pos",
                tooltip = "Set Window pos when minimize"
            };
            setPosBtn.clicked += () =>
            {
                var setPosWindow = GetWindow<SetPosWindow>() as SetPosWindow;
                setPosWindow.Init(pos => minimizePos = pos);
                setPosWindow.Focus();
            };

            topContainer.Add(windowNameLabel);
            topContainer.Add(selectBtn);
            topContainer.Add(refreshBtn);
            topContainer.Add(clearBtn);
            topContainer.Add(setPosBtn);
            return topContainer;
        }

        private void CreateWindowBtnContainer()
        {
            windowBtnContainer = new VisualElement
            {
                style =
                {
                    display = DisplayStyle.Flex,
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap
                }
            };
        }
        

        private void OnDisable()
        {
        }

        private void Update()
        {
            //update window btn color
            foreach (var windowInfo in windowInfos)
            {
                UpdateWindowBtnStyle(windowInfo.btn,windowInfo);
            }
            
            UpdateSelectBtnColor();

            var focusWindow = EditorWindow.focusedWindow;
            if (focusWindow == null)
            {
                return;
            }

            //show current focus window name
            windowNameLabel.text = focusWindow.GetType().Name;
            if (_isSelecting && focusWindow != this)
            {
                _isSelecting = false;
                AddNewWindow(focusWindow);
                UpdateView();
            }
        }

        private void UpdateSelectBtnColor()
        {
            selectBtn.style.backgroundColor = _isSelecting ? Color.green : Color.grey;
        }

        private void AddNewWindow(EditorWindow window)
        {
            if (windowInfos.FirstOrDefault(w => w.window == window) is not null)
            {
                Debug.Log($"{window.GetType().Name} is already added");
                return;
            }
            
            WindowInfo info = new WindowInfo()
            {
                window = window,
                name = window.GetType().Name,
                position = window.position,
                minSize = window.minSize,
                isMinimize = false,
                windowType = window.GetType().ToString(),
                assemblyFullName = window.GetType().Assembly.FullName
            };
            windowInfos.Add(info);
        }

        //delete all btn and recreate new by list
        private void UpdateView()
        {
            windowBtnContainer.Clear();
            foreach (var windowInfo in windowInfos)
            {
                var divider = new VisualElement();
                divider.style.width = 2;
                windowBtnContainer.Add(divider);
                CreateWindowBtn(windowInfo);
            }
        }

        private void CreateWindowBtn(WindowInfo windowInfo)
        {
            var itemContainer = new VisualElement();
            itemContainer.style.flexDirection = FlexDirection.Row;
            itemContainer.style.height = 30;

            var deleteBtn = new Button();
            deleteBtn.text = "X";
            deleteBtn.clicked += () =>
            {
                MaximizeWindow(windowInfo);
                windowInfos.Remove(windowInfo);
                UpdateView();
            };

            var btn = new Button();
            btn.style.width = 120;

            btn.text = windowInfo.name.Replace("Window", "");
            btn.clicked += () =>
            {
                if (windowInfo.window == null)
                {
                    Debug.Log($"Window {windowInfo.windowType} is closed, Create one");
                    var type = Type.GetType(windowInfo.windowType + "," + windowInfo.assemblyFullName);
                    windowInfo.window = GetWindow(type);
                    windowInfo.isMinimize = false;
                    return;
                }

                if (windowInfo.isMinimize)
                {
                    MaximizeWindow(windowInfo);
                }
                else
                {
                    MinimizeWindow(windowInfo);
                }
            };

            RemoveMarginAndBorderStyle(btn);
            RemoveMarginAndBorderStyle(deleteBtn);
            itemContainer.Add(btn);
            itemContainer.Add(deleteBtn);
            windowBtnContainer.Add(itemContainer);

            windowInfo.btn = btn;
        }

        private void RemoveMarginAndBorderStyle(VisualElement element)
        {
            element.style.marginBottom = 0;
            element.style.marginTop = 0;
            element.style.marginLeft = 0;
            element.style.marginRight = 0;
            element.style.borderBottomWidth = 0;
            element.style.borderTopWidth = 0;
            element.style.borderLeftWidth = 0;
            element.style.borderRightWidth = 0;
            element.style.borderBottomRightRadius = 0;
            element.style.borderBottomLeftRadius = 0;
            element.style.borderTopRightRadius = 0;
            element.style.borderTopLeftRadius = 0;
        }

        private void MinimizeWindow(WindowInfo windowInfo)
        {
            if (windowInfo.isMinimize)
            {
                return;
            }

            var window = windowInfo.window;

            windowInfo.isMinimize = true;
            windowInfo.position = window.position;
            windowInfo.minSize = window.minSize;

            window.minSize = new Vector2(10, 10);
            window.position = minimizePos;
        }

        private void MaximizeWindow(WindowInfo windowInfo)
        {
            if (!windowInfo.isMinimize)
            {
                return;
            }

            var window = windowInfo.window;

            windowInfo.isMinimize = false;
            window.minSize = windowInfo.minSize;
            window.position = windowInfo.position;

            window.Focus();
        }

        private void UpdateWindowBtnStyle(Button btn,WindowInfo windowInfo)
        {
            btn.style.borderBottomWidth = 3;
            if (windowInfo.isClosed)
            {
                btn.style.borderBottomColor = closeColor;
            }else if (!windowInfo.isMinimize)
            {
                btn.style.borderBottomColor = maximizeColor;
            }
            else
            {
                btn.style.borderBottomColor = minimizeColor;
            }
        }
    }

    public class SetPosWindow : EditorWindow
    {
        private Button _btn;
        private Label _infoLabel;

        public void Init(Action<Rect> onSet)
        {
            _btn = new Button();
            _btn.text = "Set";
            _btn.clicked += () =>
            {
                onSet?.Invoke(this.position);
                this.Close();
            };
            this.minSize = new Vector2(10, 10);
            rootVisualElement.Add(_btn);

            _infoLabel = new Label();
            rootVisualElement.Add(_infoLabel);
        }

        private void Update()
        {
            _infoLabel.text = position.ToString();
        }
    }
}
