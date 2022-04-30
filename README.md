ノーツのモードを変更するだけ

AlternativePlayとNalulunaModifierを組み合わせた使い方に、当たり判定がおかしくなったりするのを解消する

## 依存Mod

NoteMode 0.2.0以降は以下のModに依存しています

- SiraUtil

### BS 1.19.x　までの機能

NoteMode 0.0.9までは下記内容のものが利用できます。

- Remove Red: 赤ノーツ削除（NalulunaModifierから移植）AlternativePlayで利用できなかったため
- Remove Blue: 青ノーツ削除（同上）
- One Color Red: 全てのノーツを赤にする（AlternativePlayのOneColorとNalulunaModifierの4saberモードを組み合わせたときに当たり判定が無くなるため）
- One Color Blue: 全てのノーツを青にする（同上）
- No Arrows: 全てのノーツをドットに変更する
- Remove Notes Bomb: ノーツとボムを抹消するモード（壁譜面の壁だけ見たかっただけ）
- Reverse Arrows: 全てのノーツ向きを反転させる（簡単な譜面でプレイすること！）
- Randomize Arrows（ノーツの向きがランダムに変更されます）
- Restricted Randomize Arrows（制限付きでノーツの向きがランダムに変更されます）
- Note Scale: ノーツサイズを変更する（当たり判定も変更されます）

### BS 1.20.0以降に追加された機能

NoteMode 0.2.0以降は下記内容のものが追加されています。

- All Notes BurstSliderHead: 全てのノーツをチェインノーツの頭部分のみにします
- All Arc Mode: 基本的には全てにアークが生成されます（各モードによって生成する処理が違います）
- Restricted Arc Mode: 制限付きのアーク生成（色が交互に発生する場合は生成しません）
- Rainbow Color: ノーツがカラフルになります。（Noodle ExtensionsとChromaが両方使われた譜面の場合は変更しないようにしました）

Randomize Arrows、Restricted Randomize Arrowsの機能は、[ふぁずぱい](https://twitter.com/FaZ_Pi)さんによって作成されました。

![NoteMode](https://raw.githubusercontent.com/nullpon16tera/nullpon16tera.github.io/master/NoteMode/note_mode.png "NoteMode Modifier")
