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

## 명령

```
./cshaheui <입력 파일> [compile [bigint] | binary [bigint]]
```

옵션 없이 실행하면 컴파일하지 않고 실행하게 됩니다.

* `compile`: .NET 어셈블리로 컴파일한 뒤 실행합니다.
* `binary`: .NET 어셈블리로 컴파일한 뒤 실행 파일을 작성합니다. `out.exe`로 저장됩니다.
* `bigint`: `int` 대신 `BigInteger`를 사용합니다. `compile` 또는 `binary` 옵션과 함께 사용합니다.

## 성능

Git Bash에 입력한 명령입니다.

```
$ time ./cshaheui logo/logo.aheui > logo.out

real    3m47.205s
user    0m0.000s
sys     0m0.015s
```

[로고](https://github.com/aheui/snippets/blob/master/logo/logo.aheui)입니다. i7 스카이레이크인데 227초군요...

## 컴파흴러

그냥 실행하면 상당히 느립니다. 하지만 코드를 컴파일하면 어떨까요?

```
$ time ./cshaheui logo/logo.aheui compile > logo.out

real    1m54.786s
user    0m0.000s
sys     0m0.015s
```

114초입니다. 속도가 2배 빨라진 것을 확인할 수 있습니다. 만약 미리 컴파일해 둔 바이너리를 실행한다면?

```
$ ./cshaheui logo/logo.aheui binary
$ time ./out > logo.out

real    1m9.803s
user    0m0.000s
sys     0m0.015s
```

70초 정도 걸려서 로고를 찍어내는 것을 확인할 수 있습니다.

```
$ ./cshaheui aheui.aheui binary
$ time ./out < quine/quine.puzzlet.40col.aheui
상밢밢밣밦발받밧밥밣밦밦받밦밢밝받밝받밦밧밢받발받밧밣밦밥발받밝밥밧밦밦받밧받붑

...

뫃떠벌번정따도퍼즐릿
real    0m14.320s
user    0m0.000s
sys     0m0.015s
```

[아희아희](https://github.com/aheui/aheui.aheui)를 컴파일한 뒤 40col 콰인을 돌려 봤습니다. 14초입니다. 한 칸씩 따라가며 실행했을 때는 55초 정도 걸립니다.

## TODO
* 컴파흴러 개선
* 한 칸씩 실행할 때도 `BigInteger`를 선택할 수 있도록 수정
