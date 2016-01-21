# 씨샤희

C#으로 짠 아희 구현체입니다.

* **최적화되지 않았습니다.**
* UTF-8 입력, UTF-8 출력. 윈도우 콘솔에서는 깨져 보일 수 있습니다. 입출력을 리디렉션해 주세요.
* 리눅스에서는 테스트해보지는 않았는데, 적당한 컴파일러를 쓰면 분명 작동할 것이라고 믿습니다.
* [테스트 수트](https://github.com/aheui/snippets/tree/master/standard) 통과
* [40col 콰인](https://github.com/aheui/snippets/blob/master/quine/quine.puzzlet.40col.aheui) 정상작동

## 특징

* ㅎ 저장공간은 통로입니다만, 지금 구현으로는 `/dev/null`처럼 행동하게 됩니다.
* EOF에 다다르면 문자 입력 결과가 -1을 반환합니다.

## 성능

Git Bash에 입력한 명령입니다.

```
$ time ./cshaheui logo/logo.aheui > logo.out

real    4m47.139s
user    0m0.000s
sys     0m0.015s
```

[로고](https://github.com/aheui/snippets/blob/master/logo/logo.aheui)입니다. i7 스카이레이크인데 287초군요... 이게 다 `BigInteger` 때문입니다.

```
$ time ./cshaheui aheui.aheui < quine/quine.puzzlet.40col.aheui
상밢밢밣밦발받밧밥밣밦밦받밦밢밝받밝받밦밧밢받발받밧밣밦밥발받밝밥밧밦밦받밧받붑

...

뫃떠벌번정따도퍼즐릿
real    0m55.494s
user    0m0.000s
sys     0m0.015s
```

[아희아희](https://github.com/aheui/aheui.aheui)로 40col 콰인을 돌려 봤습니다. 55초입니다. 장난 아니네요. 로고를 돌리면 어떨까요...

## TODO
* `BigInteger`와 `int`를 선택할 수 있는 옵션 추가
* 할 수 있을지는 모르겠지만, 아희 코드를 IL으로 컴파일
