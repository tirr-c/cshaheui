# 씨샤희

C#으로 짠 아희 구현체입니다.

* UTF-8 입력, UTF-8 출력. 윈도우 콘솔에서는 깨져 보일 수 있습니다. 입출력을 리디렉션해 주세요.
* 리눅스에서는 테스트해보지는 않았는데, 적당한 컴파일러를 쓰면 분명 작동할 것이라고 믿습니다.
* [테스트 수트](https://github.com/aheui/snippets/tree/master/standard) 통과
* [40col 콰인](https://github.com/aheui/snippets/blob/master/quine/quine.puzzlet.40col.aheui) 정상작동

## 성능

Git Bash에 입력한 명령입니다.

```
$ time ./shaheui.exe logo/logo.aheui > out.txt

real    3m18.846s
user    0m0.015s
sys     0m0.062s
```

[로고](https://github.com/aheui/snippets/blob/master/logo/logo.aheui)입니다. i5 아이비브릿지에서 200초 정도 걸립니다.

```
$ time ./shaheui.exe /f/aheui.aheui < quine/quine.puzzlet.40col.aheui
상밢밢밣밦발받밧밥밣밦밦받밦밢밝받밝받밦밧밢받발받밧밣밦밥발받밝밥밧밦밦받밧받붑

...

뫃떠벌번정따도퍼즐릿
real    0m43.249s
user    0m0.031s
sys     0m0.031s
```

[아희아희](https://github.com/aheui/aheui.aheui)로 40col 콰인을 돌려 봤습니다. 43초입니다. 장난 아니네요. 로고를 돌리면 어떨까요...
