using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Reader : MonoBehaviour
{

	public TextMesh _textOut;
	public TextMesh _textName;
	public int _sumbolInStr; //вывод кол-ва символов в строке

	private string _allText; //весь текст
	private string _currText; //обрабатываемый текст
	private string _textForRead; //текст вывода на экран
	private string _texsForSelect; //текст выбора персонажем

	private int _stopInAll; //позиция прочитанного
	private int _stopInRead;
	private int _currStartIndex; //позиция старта текста выборки
	private int _currEndIndex; //позиция конца текста выборки

	private bool _select; //выбор персонажем
	private List<string> _listSelect = new List<string>(); //перечень текста выбора 
	private string _currSelect;

	// Use this for initialization
	void Start ()
	{
		TextAsset testText = (TextAsset)Resources.Load("textCharter", typeof(TextAsset)); //чтение файла
		_allText = testText.text; 
		Scan(_allText);
		ReadString();
	}

	private void Scan(string testStr)
	{
		//осуществление анализа для определения выводимого текста и служебного
		if (testStr.Contains("StartChSelect"))
		{
			_currStartIndex = testStr.IndexOf("StartChSelect");
			_currEndIndex = testStr.IndexOf("EndChSelect");
			int length = _currEndIndex + 11 - _currStartIndex;
			_texsForSelect = testStr.Substring(_currStartIndex, length);
			_textForRead = testStr.Remove(_currStartIndex);
		}
		else
		{
			//здесь ещё будет переход если будет добавленно
			_textForRead = testStr;
		}
//		_textForRead = _textForRead.Replace("\n", " "); //замена переносов строки в файле на пробелы
	}

	private void Switch()
	{
		//выбор блока при текущем выборе сюжета
		_listSelect.Clear(); //очистка предыдущих выборов


		_currStartIndex = _allText.IndexOf("Select " + _currSelect); 
		_currEndIndex = _allText.IndexOf("EndSelect " + _currSelect);
		int length = _currEndIndex - _currStartIndex;
		int start = "Select #.#".Length;
		int end = "EndSelect #.#".Length;
		_currText = _allText.Substring(_currStartIndex + start,length - end);
		Scan(_currText);

		_stopInAll += _stopInRead;//
		_stopInRead = 0;//
	}

//	private string Header(string testStr)
//	{
//		if (testStr.Contains("NameChar"))
//		{
//			int startName = testStr.IndexOf("NameChar");
//			
//			if (startName <= 1)
//			{
//				int endName = testStr.IndexOf(':');
//				int length = endName - startName;
//				_textName.text = testStr.Substring(0, length + 1);
//				_stopInRead += length + 1;
//				return testStr.Substring(length + 1);
//			}
//			else
//			{
////				_stopInRead -= testStr.Substring(0, startName).Length;
//				return testStr.Substring(0, startName);
//			}
//			
//
//		}
//		else
//		{
//			return testStr;
//		}
//	}

	private void ReadString()
	{
		//чтение строки для вывода на экран
		if(_stopInRead == _textForRead.Length - 1) return;

		string output = null; //строка на вывод

		for (int i = 0; i < 2; i++)
		{
			
			string buffStr = null; //строка для преобразований
			
			if (_textForRead.Length - _stopInRead > _sumbolInStr) //проверка на последние выводимые символы
			{
				buffStr = _textForRead.Substring(_stopInRead, _sumbolInStr); //содержние одной строки
				buffStr = buffStr.Replace("\n", " ");
//				buffStr = Header(buffStr);

				if (buffStr[buffStr.Length - 1] != ' ')
				{
					buffStr = BackToEscape(buffStr);
				}
				_stopInRead += buffStr.Length;
				output += buffStr + "\n";
				
			}
			else
			{
				buffStr = _textForRead.Substring(_stopInRead);
				buffStr = buffStr.Replace("\n", " ");
//				buffStr = Header(buffStr);

				_stopInRead = _textForRead.Length-1;
				output += buffStr;
				break;
			}
		}
		_textOut.text = output;
	}

	private string BackToEscape(string buffStr)
	{
		//предотвращение переноса при разрыве слова
		for (int i = buffStr.Length - 1; i > 0; i--)
		{
			if (buffStr[i] == ' ')
			{
				buffStr = buffStr.Substring(0, i + 1); //пробел оставить на той же строке заменяется предидущее содержимое исправть
//				_stopInRead -= i + 1;
				break;
			}
		}
		return buffStr;
	}

	private void ForSelect()
	{
		int start = "StartChSelect №\n".Length + 1;
		int end = "EndChSelect".Length + start;

		_currSelect = _texsForSelect.Substring(14, 1);

		_texsForSelect = _texsForSelect.Substring(start, _texsForSelect.Length - end);

		int count = 0;

		foreach (var ch in _texsForSelect)
		{
			if (ch == '\n') count++;
		}

		for (int i = 0; i < count; i++)
		{
			int index = _texsForSelect.IndexOf('\n');
			_listSelect.Add(_texsForSelect.Substring(0, index)); //записываем в лист варианты выбора
			_texsForSelect = _texsForSelect.Remove(0, index+1);
		}
		
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			if (_stopInRead + 1 == _currStartIndex)
			{
				if(_select) return;
				ForSelect();
				_select = true;
			}
			else
			{
				ReadString();
			}
		}
	}

	void OnGUI()
	{
		if (_select)
		{
			for (int i = 0; i < _listSelect.Count; i++)
			{
				if (GUI.Button(new Rect(100, i*100, 200, 50), _listSelect[i]))
				{
					_currSelect +="." + (i+1).ToString();
					Switch();
					_select = false;
				}
			}
		}
	}
}
