// ==================================================
// GlobalDebugDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace Dialog
{
    public class GlobalDebugDialog : IDialog
    {
        [SerializeField] private GameObject _resultObj;
        [SerializeField] private Button _button;
        [SerializeField] private Text _text;
        [SerializeField] private Text _debugResult;
        private bool _isDebugMode = false;
        private float _deltaTime = 0f;
        float total;
        float alloc;
        float unused;

        protected override void OnLoad()
        {
            base.OnLoad();

            _isDebugMode = false;

            _button.onClick.AddListener(OnClickButton);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _button.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void OnClickButton()
        {
            _isDebugMode = !_isDebugMode;

            if (_isDebugMode == true)
            {
                _text.text = "MEM\nOFF";
                _resultObj.SetActive(true);
            }
            else
            {
                _text.text = "MEM\nOn";
                _resultObj.SetActive(false);
            }

            _debugResult.text = string.Empty;
        }

        private void Update()
        {
            if (_isDebugMode == false)
                return;

            total = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
            alloc = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;
            unused = Profiler.GetTotalUnusedReservedMemoryLong() / 1024f / 1024f;
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            _debugResult.text = string.Format("[RAM Info]\nTotal= {0}\nalloc= {1}\nunused= {2}\n\n[FPS] {3:0.0}ms {4:0.0}fps", total, alloc, unused, _deltaTime * 1000f, 1f / _deltaTime);
        }
    }
}