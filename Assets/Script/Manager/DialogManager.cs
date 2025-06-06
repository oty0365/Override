using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : HalfSingleMono<DialogManager>
{

    [Header("다이얼로그 데이터")]
    public Dialogs currentDialogs;
    public int currentIndex;
    public bool isEventing;

    [Header("다이얼로그 UI")]
    public GameObject dialogPannel;
    public Image talkerPortraitImage;
    public TextMeshProUGUI talkerNameTmp;
    public TextMeshProUGUI talkerDialogTmp;
    public GameObject nextIndicator;

    [Header("텍스트 출력 속도")]
    [SerializeField] private float textSpeed = 0.05f;

    private bool _isPuttingText;
    private Coroutine _putTextFlow;
    private Scripter scripter;

    private void Start()
    {
        dialogPannel.SetActive(false);
        if (nextIndicator != null)
        {
            nextIndicator.SetActive(false);
        }
        scripter = Scripter.Instance;
    }

    private void Update()
    {
        if (!dialogPannel.activeSelf) return;

        if (nextIndicator != null)
            nextIndicator.SetActive(!_isPuttingText);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            NextText();
        }
    }

    public void StartConversation(Dialogs dialogs)
    {
        dialogPannel.SetActive(true);
        currentDialogs = dialogs;
        currentIndex = 0;
        _isPuttingText = false;
        PutText();
    }

    public void NextText()
    {
        if (!isEventing)
        {

            if (_isPuttingText)
            {
                StopCoroutine(_putTextFlow);
                talkerDialogTmp.text = scripter.scripts[currentDialogs.dialogScripts[currentIndex].dialogue[0]].currentText;
                _isPuttingText = false;
                return;
            }

            currentIndex++;

            if (currentIndex >= currentDialogs.dialogScripts.Length)
            {
                EndConversation();
                return;
            }

            PutText();
        }
    }

    private void EndConversation()
    {
        dialogPannel.SetActive(false);
        currentDialogs = null;
        currentIndex = 0;

        PlayerInteraction.Instance.OnInteractMode(1);

        if (nextIndicator != null)
            nextIndicator.SetActive(false);
    }

    private void PutText()
    {
        var currentDia = currentDialogs.dialogScripts[currentIndex];
        if (currentDia.eventsWhileTalk.Length > 0)
        {
            isEventing = true;
            foreach(var i in currentDia.eventsWhileTalk)
            {
                GameEventManager.Instance.eventsDict[i].Invoke();
            }
        }
        else
        {
            _isPuttingText = true;
            talkerPortraitImage.sprite = currentDia.talkersFace;
            talkerNameTmp.text = scripter.scripts[currentDia.talker].currentText;
            _putTextFlow = StartCoroutine(PutTextFlow(scripter.scripts[currentDia.dialogue[0]].currentText));
        }
    }

    private IEnumerator PutTextFlow(string dialog)
    {
        talkerDialogTmp.text = "";

        dialog = dialog.Replace("\\n", "\n");

        int i = 0;
        while (i < dialog.Length)
        {
            if (dialog[i] == '<')
            {
                int tagEnd = dialog.IndexOf('>', i);
                if (tagEnd != -1)
                {
                    string tag = dialog.Substring(i, tagEnd - i + 1);
                    talkerDialogTmp.text += tag;
                    i = tagEnd + 1;
                    continue;
                }
            }

            if (dialog[i] == '\n')
            {
                talkerDialogTmp.text += '\n';
                i++;
                continue;
            }

            talkerDialogTmp.text += dialog[i];
            i++;

            yield return new WaitForSeconds(textSpeed);
        }

        _isPuttingText = false;
    }

}
