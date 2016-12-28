package y2016.R1A.B

import cmn._

import scala.collection.mutable

object RankAndFileSimple extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    val n = fetch(PInt)

    out((0 until (2 * n - 1)).flatMap(i => fetch(PList(PInt))).groupBy(n => n).toList.filter({case (_, nums) => nums.size % 2 == 1}).map(_._1).sorted:_*)
  }
}
