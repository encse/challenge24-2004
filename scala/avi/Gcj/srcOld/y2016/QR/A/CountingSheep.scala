package y2016.QR.A

import cmn._

import scala.collection.mutable

object CountingSheep extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) : Unit = {

    val num = fetch(PBigInt)

    if(num == 0) {
      out("INSOMNIA")
      return
    }

    //bigNums(1).takeWhile(_ < 1000000).foreach(num => {

      //print(s"$num: ")
      val ns = nums(BigInt(1)).map(_*num)
      def col(ns: Stream[BigInt], hlm: Set[Int] = Set()): Stream[(BigInt,Set[Int])] = ns match {
        case n #:: tail => {
          val hlmNext = hlm ++ n.toString().map(_.toInt)
          (n, hlmNext) #:: col(tail, hlmNext)
        }
      }

      val res = col(ns).filter({case (n, hlm) => hlm.size == 10}).head._1
      //println(res)
    //})
    out(res)
  }
}
