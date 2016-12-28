package y2016.R1A.A

import cmn._

import scala.collection.mutable

object TheLastWord extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    val word = fetch(PString)
    val rgchWord = word.map(ch => ch.toInt).toArray

    var rgchBegin = mutable.ArrayBuffer[Int]()
    var rgchEnd = Array[Int]()

    var rgch = rgchWord

    while (rgch.nonEmpty) {

      var chMax = rgch.head
      var iMax = 0

      for ((ch, i) <- rgch.zipWithIndex) {
        if (ch >= chMax) {
          chMax = ch
          iMax = i
        }
      }

      rgchBegin += chMax
      rgchEnd = rgch.slice(iMax + 1, rgch.length) ++ rgchEnd
      rgch = rgch.slice(0, iMax)
    }

    val res = rgchBegin.map(ch => ch.toChar).mkString + rgchEnd.map(ch => ch.toChar).mkString
    assert(rgchWord.length == res.length)
    out(res)
  }
}
