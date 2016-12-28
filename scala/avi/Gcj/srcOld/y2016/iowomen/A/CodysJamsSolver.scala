package y2016.iowomen.A

import cmn._

import scala.collection.immutable._

object CodysJamsSolver extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {
    val n = fetch(PInt)

    def trans(rg: List[Int], qu: Queue[Int]): List[Int] = rg match {
      case Nil => {
        assert(qu.isEmpty)
        Nil
      }
      case num :: nums =>
        if (qu.headOption.contains(num))
          trans(nums, qu.tail)
        else
          num :: trans(nums, qu.enqueue(num / 3 * 4))
    }

    val rgin = fetch(PList(PInt))
    assert(rgin.length == n * 2)

    val rgout = trans(rgin, Queue())
    assert(rgout.length == n)
    assert(rgout.flatMap(x => List(x, x / 3 * 4)).sorted.zip(rgin).forall({ case (a, b) => a == b }))
    rgout.foreach(out(_))
  }
}
